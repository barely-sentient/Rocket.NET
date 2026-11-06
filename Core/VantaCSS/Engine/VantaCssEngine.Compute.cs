using System;
using Rocket.Core.DOM;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Linq;

namespace VantaCSS
{
    /// <summary>
    /// Style computation (pseudo-classes + transitions) for <see cref="VantaCssEngine"/>.
    /// </summary>
    public partial class VantaCssEngine
    {
        private sealed class PropIds
        {
            public FieldInfo IdField;
            public PropertyInfo IdProp;
            public FieldInfo ClassField;
            public PropertyInfo ClassProp;
        }

        private static readonly ConcurrentDictionary<Type, PropIds> _propIdCache = new ConcurrentDictionary<Type, PropIds>();

        private static PropIds GetPropIds(Type t)
        {
            return _propIdCache.GetOrAdd(t, type =>
            {
                var p = new PropIds();
                p.IdField = type.GetField("Id") ?? type.GetField("id");
                p.IdProp = type.GetProperty("Id") ?? type.GetProperty("id");
                p.ClassField = type.GetField("ClassName") ?? type.GetField("className");
                p.ClassProp = type.GetProperty("ClassName") ?? type.GetProperty("className");
                return p;
            });
        }

        private const int MaxRuleMatchCacheEntries = 1024;

        private readonly ConcurrentDictionary<string, List<CssRule>> _ruleMatchCache =
            new ConcurrentDictionary<string, List<CssRule>>();

        private List<CssRule> GetMatchingRules(string tag, string id, string className, Node node, int width, int height, Orientation orientation)
        {
            string pseudoKey = node == null
                ? string.Empty
                : $"{node.Hovered}:{node.Focused}:{node.Active}:{node.Visited}:{node.Checked}:{node.Disabled}:{node.PseudoState}";
            string ancestorKey = GetAncestorKey(node);
            string key = $"{tag}|{id}|{className}|{width}|{height}|{orientation}|{pseudoKey}|{ancestorKey}|{Stylesheets.Count}|{_stylesheetVersion}";

            if (_ruleMatchCache.Count >= MaxRuleMatchCacheEntries) _ruleMatchCache.Clear();
            return _ruleMatchCache.GetOrAdd(key, _ => BuildMatchingRules(tag, id, className, node, width, height, orientation));
        }

        private static string GetAncestorKey(Node node)
        {
            if (node == null || node.Parent == null) return string.Empty;
            var sb = new StringBuilder();
            var current = node.Parent;
            while (current != null)
            {
                string tag, id, className;
                VantaCssEngine.GetNodeIdentity(current, out tag, out id, out className);
                sb.Append(tag).Append(':').Append(id).Append(':').Append(className).Append(';');
                current = current.Parent;
            }
            return sb.ToString();
        }

        private List<CssRule> BuildMatchingRules(string tag, string id, string className, Node node, int width, int height, Orientation orientation)
        {
            var collected = new List<(CssRule rule, int specificity, int order)>();
            int order = 0;
            foreach (var sheet in Stylesheets)
            {
                foreach (var rule in sheet.Rules)
                {
                    if (!MatchesMedia(rule.Media, width, height, orientation)) continue;
                    if (!SelectorMatches(rule.Selector, tag, id, className, node, this)) continue;
                    collected.Add((rule, MediaSpecificity(rule.Media), order++));
                }
            }
            // Order-independent media-query precedence: more specific (narrower) media
            // queries win over broader ones regardless of source order.
            collected.Sort((a, b) => a.specificity != b.specificity
                ? a.specificity.CompareTo(b.specificity)
                : a.order.CompareTo(b.order));
            var result = new List<CssRule>(collected.Count);
            foreach (var x in collected) result.Add(x.rule);
            return result;
        }

        private int MediaSpecificity(string media)
        {
            if (string.IsNullOrWhiteSpace(media)) return 0;
            var queries = VantaCSS.Grammar.Grammar.ParseMediaQueries(media);
            if (queries == null) return 0;
            int score = 0;
            // max-*: smaller => more specific. min-*: larger => more specific.
            foreach (var query in queries)
            {
                foreach (var feature in query.Features)
                {
                    bool isMax = feature.Name == "max-width" || feature.Name == "max-height";
                    bool isMin = feature.Name == "min-width" || feature.Name == "min-height";
                    if (!isMax && !isMin) continue;
                    double px = ParseLen(feature.Value, 0);
                    if (double.IsNaN(px)) continue;
                    int v = (int)px;
                    score += isMax ? 10000 - v : v;
                }
            }
            return score;
        }

        // -----------------------------------------------------------------
        // Time-aware computation entry points
        // -----------------------------------------------------------------

        public string ComputeStyle(Rocket.Core.DOM.Node node, int width, int height, Orientation orientation, double time)
        {
            if (node == null) return string.Empty;

            // Gather node identity.
            string tag = null;
            try { tag = node.Type != null ? node.Type.Name : null; } catch { }
            var props = node.UntypedProps;
            string id = null, className = null;
            if (props != null)
            {
                var ids = GetPropIds(props.GetType());
                id = ids.IdField?.GetValue(props) as string ?? ids.IdProp?.GetValue(props) as string;
                className = ids.ClassField?.GetValue(props) as string ?? ids.ClassProp?.GetValue(props) as string;
            }

            string pseudoKey = $"{tag}|{id}|{className}|{node.Hovered}:{node.Focused}:{node.Active}:{node.Visited}:{node.Checked}:{node.Disabled}:{node.PseudoState}|{GetAncestorKey(node)}";
            if (node._styleCacheValid &&
                node._styleCacheW == width &&
                node._styleCacheH == height &&
                node._styleCacheO == orientation &&
                node._styleCacheTime == time &&
                node._styleCachePseudoKey == pseudoKey &&
                node._styleCacheSheetVersion == _stylesheetVersion)
            {
                return node._styleCacheResult;
            }

            // Collect matching declarations into an ordered target dictionary.
            var target = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var pseudoTargets = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["::before"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                ["::after"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            };
            var transitionSpecs = new List<CssTransition>();

            var matchingRules = GetMatchingRules(tag, id, className, node, width, height, orientation);
            // Global :root custom properties apply everywhere.
            foreach (var sheet in Stylesheets)
            {
                foreach (var r in sheet.Rules)
                {
                    // skip at-rules (they have no direct declarations we need here)
                    if (!MatchesMedia(r.Media, width, height, orientation)) continue;
                    if (!string.Equals(r.Selector?.Trim(), ":root", StringComparison.OrdinalIgnoreCase)) continue;
                    foreach (var kv in r.Declarations)
                    {
                        if (kv.Key.StartsWith("--", StringComparison.Ordinal) && !target.ContainsKey(kv.Key))
                            target[kv.Key] = kv.Value;
                    }
                }
            }
            foreach (var rule in matchingRules)
            {
                string sel = rule.Selector ?? string.Empty;
                bool isBefore = sel.IndexOf("::before", StringComparison.OrdinalIgnoreCase) >= 0;
                bool isAfter = sel.IndexOf("::after", StringComparison.OrdinalIgnoreCase) >= 0;
                if (isBefore || isAfter)
                {
                    var pseudoName = isBefore ? "::before" : "::after";
                    if (!pseudoTargets.ContainsKey(pseudoName))
                        pseudoTargets[pseudoName] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var kv in rule.Declarations)
                    {
                        pseudoTargets[pseudoName][kv.Key] = kv.Value; // last wins
                    }
                    var pspecs = ParseTransitions(rule.Declarations);
                    if (pspecs.Count > 0) transitionSpecs = pspecs;
                    continue;
                }

                foreach (var kv in rule.Declarations)
                {
                    target[kv.Key] = kv.Value; // last wins
                }
                // transition specs from this rule
                var specs = ParseTransitions(rule.Declarations);
                if (specs.Count > 0)
                {
                    // Replace or append? Last rule wins for transition property.
                    transitionSpecs = specs;
                }
            }

            // Inherit CSS custom properties from parent.
            if (node.Parent != null && node.Parent.ComputedStyles != null)
            {
                foreach (var kv in node.Parent.ComputedStyles)
                {
                    if (kv.Key.StartsWith("--", StringComparison.Ordinal) && !target.ContainsKey(kv.Key))
                        target[kv.Key] = kv.Value;
                }
                foreach (var kv in node.Parent.ComputedStyles)
                {
                    if (IsInheritable(kv.Key) && !target.ContainsKey(kv.Key))
                        target[kv.Key] = kv.Value;
                }
            }

            // Resolve var() references in target values.
            var tkeys = new List<string>(target.Keys);
            foreach (var k in tkeys)
                target[k] = ResolveVarValue(target[k], node, target);

            // Resolve var() references in pseudo-element target values.
            foreach (var pk in new List<string>(pseudoTargets.Keys))
            {
                var pt = pseudoTargets[pk];
                // Variables such as --fa are defined on the element's main target,
                // so resolve against a merged variable context.
                var varContext = new Dictionary<string, string>(target, StringComparer.OrdinalIgnoreCase);
                foreach (var kv in pt) varContext[kv.Key] = kv.Value;
                var pkeys = new List<string>(pt.Keys);
                foreach (var k in pkeys)
                    pt[k] = ResolveVarValue(pt[k], node, varContext);
            }

            // Also collect transition specs from the merged target (in case transition declared as a normal decl).
            var mergedSpecs = ParseTransitions(target);
            if (mergedSpecs.Count > 0) transitionSpecs = mergedSpecs;

            // Manage transition state.
            var st = GetTransitionState(node);

            var oldTarget = st.Target;
            var oldComputed = st.Computed;

            // Determine which properties transition.
            Func<string, CssTransition> findSpec = (prop) =>
            {
                foreach (var sp in transitionSpecs)
                {
                    if (string.Equals(sp.Property, "all", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(sp.Property, prop, StringComparison.OrdinalIgnoreCase))
                        return sp;
                }
                return null;
            };

            // Update active transitions.
            var computed = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Properties in new target.
            foreach (var kv in target)
            {
                string prop = kv.Key;
                string newVal = kv.Value;
                string oldVal = null;
                if (oldTarget != null && oldTarget.TryGetValue(prop, out oldVal)) { }
                else if (oldComputed != null && oldComputed.TryGetValue(prop, out oldVal)) { }

                var spec = findSpec(prop);

                bool hasActive = st.ActiveTransitions.TryGetValue(prop, out var existingPt) && existingPt != null && existingPt.Active;

                if (hasActive && string.Equals(existingPt.ToValue, newVal, StringComparison.OrdinalIgnoreCase))
                {
                    // Keep existing transition.
                }
                else if (oldVal != null && !string.Equals(oldVal, newVal, StringComparison.OrdinalIgnoreCase) && spec != null && spec.Duration > 0)
                {
                    string from;
                    if (hasActive) from = existingPt.FromValue;
                    else if (oldComputed != null && oldComputed.TryGetValue(prop, out var oc)) from = oc;
                    else from = oldVal ?? newVal;
                    // If a transition is in flight, transition from current displayed value.
                    if (hasActive)
                    {
                        // compute current displayed
                        // (best effort: use oldComputed)
                        if (oldComputed != null && oldComputed.TryGetValue(prop, out var cur)) from = cur;
                    }
                    var pt = new PropertyTransition
                    {
                        Property = prop,
                        FromValue = from,
                        ToValue = newVal,
                        StartTime = time,
                        Duration = spec.Duration,
                        Delay = spec.Delay,
                        TimingFunction = spec.TimingFunction,
                        Active = true
                    };
                    st.ActiveTransitions[prop] = pt;
                }
                else if (spec != null && spec.Duration > 0 && oldVal == null)
                {
                    // New property: snap (no from value).
                    st.ActiveTransitions.Remove(prop);
                }
                else if (!hasActive)
                {
                    // No transition: snap, clear active.
                    st.ActiveTransitions.Remove(prop);
                }

                // Compute displayed value.
                if (st.ActiveTransitions.TryGetValue(prop, out var activePt) && activePt != null && activePt.Active)
                {
                    double elapsed = time - activePt.StartTime - activePt.Delay;
                    if (elapsed < 0)
                    {
                        computed[prop] = activePt.FromValue;
                    }
                    else if (activePt.Duration <= 0 || elapsed >= activePt.Duration)
                    {
                        computed[prop] = activePt.ToValue;
                        activePt.Active = false;
                        st.ActiveTransitions.Remove(prop);
                    }
                    else
                    {
                        double p = elapsed / activePt.Duration;
                        double eased = Ease(activePt.TimingFunction, p);
                        computed[prop] = Interpolate(activePt.FromValue, activePt.ToValue, eased);
                    }
                }
                else
                {
                    computed[prop] = newVal;
                }
            }

            // Properties removed from target: keep old computed? Usually property no longer applied. We'll drop them.
            // But if transition out? Skip for simplicity.

            // Save state.
            st.Target = new Dictionary<string, string>(target, StringComparer.OrdinalIgnoreCase);
            st.Computed = new Dictionary<string, string>(computed, StringComparer.OrdinalIgnoreCase);
            st.Transitions = transitionSpecs;
            st.LastTime = time;

            if (node != null) node.PseudoStyles = pseudoTargets;

            // Build output string in target order.
            var sb = new StringBuilder();
            foreach (var kv in target)
            {
                string val;
                if (!computed.TryGetValue(kv.Key, out val)) val = kv.Value;
                sb.Append(kv.Key);
                sb.Append(": ");
                sb.Append(val);
                sb.Append("; ");
            }
            var result = sb.ToString().Trim();
            node._styleCacheW = width;
            node._styleCacheH = height;
            node._styleCacheO = orientation;
            node._styleCacheTime = time;
            node._styleCachePseudoKey = pseudoKey;
            node._styleCacheResult = result;
            node._styleCacheSheetVersion = _stylesheetVersion;
            node._styleCacheValid = true;
            return result;
        }


        public string ComputeStyle(Rocket.Core.DOM.Node node, double time)
        {
            return ComputeStyle(node, ViewportWidth, ViewportHeight, Orientation, time);
        }


        // Pseudo-state convenience overloads.
        public string ComputeStyle(Rocket.Core.DOM.Node node, string pseudoState)
        {
            if (node != null && !string.IsNullOrWhiteSpace(pseudoState)) SetPseudoState(node, pseudoState);
            return ComputeStyle(node);
        }

        public string ComputeStyle(Rocket.Core.DOM.Node node, int width, int height, Orientation orientation, string pseudoState)
        {
            if (node != null && !string.IsNullOrWhiteSpace(pseudoState)) SetPseudoState(node, pseudoState);
            return ComputeStyle(node, width, height, orientation);
        }

        public string ComputeStyle(Rocket.Core.DOM.Node node, int width, int height, Orientation orientation, double time, string pseudoState)
        {
            if (node != null && !string.IsNullOrWhiteSpace(pseudoState)) SetPseudoState(node, pseudoState);
            return ComputeStyle(node, width, height, orientation, time);
        }

        public Dictionary<string, string> ComputeStyles(Rocket.Core.DOM.Node node, string pseudoState)
        {
            if (node != null && !string.IsNullOrWhiteSpace(pseudoState)) SetPseudoState(node, pseudoState);
            return ComputeStyles(node);
        }


        // Time-aware ComputeStyles
        public Dictionary<string, string> ComputeStyles(Rocket.Core.DOM.Node node, int width, int height, Orientation orientation, double time)
        {
            var style = ComputeStyle(node, width, height, orientation, time);
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var declaration in VantaCSS.Grammar.Grammar.ParseDeclarations(style))
                dict[declaration.Property] = declaration.Value;
            return dict;
        }

        public Dictionary<string, string> ComputeStyles(Rocket.Core.DOM.Node node, double time)
        {
            return ComputeStyles(node, ViewportWidth, ViewportHeight, Orientation, time);
        }

        // Time-aware Apply
        public void ApplyComputedStyles(Rocket.Core.DOM.Node root, int width, int height, Orientation orientation, double time)
        {
            if (root == null) return;
            _applyDepth++;
            var style = ComputeStyle(root, width, height, orientation, time);
            root.ComputedStyle = style;
            if (root.ComputedStyles == null) root.ComputedStyles = new Dictionary<string, string>();
            root.ComputedStyles.Clear();
            foreach (var declaration in VantaCSS.Grammar.Grammar.ParseDeclarations(style))
                root.ComputedStyles[declaration.Property] = declaration.Value;

            // Inherit CSS custom properties from parent.
            if (root.Parent != null && root.Parent.ComputedStyles != null)
            {
                foreach (var kv in root.Parent.ComputedStyles)
                {
                    if (kv.Key.StartsWith("--", StringComparison.Ordinal) && !root.ComputedStyles.ContainsKey(kv.Key))
                        root.ComputedStyles[kv.Key] = kv.Value;
                }
                foreach (var kv in root.Parent.ComputedStyles)
                {
                    if (IsInheritable(kv.Key) && !root.ComputedStyles.ContainsKey(kv.Key))
                        root.ComputedStyles[kv.Key] = kv.Value;
                }
            }

            // Resolve var() references.
            var rkeys = new List<string>(root.ComputedStyles.Keys);
            foreach (var k in rkeys)
                root.ComputedStyles[k] = ResolveVarValue(root.ComputedStyles[k], root);

            // Rebuild computed style string.
            var sb = new StringBuilder();
            foreach (var kv in root.ComputedStyles)
            {
                sb.Append(kv.Key).Append(": ").Append(kv.Value).Append("; ");
            }
            root.ComputedStyle = sb.ToString().Trim();

            foreach (var child in root.Children ?? Array.Empty<Rocket.Core.DOM.Node>())
            {
                child.Parent = root;
                ApplyComputedStyles(child, width, height, orientation, time);
            }
            if (_applyDepth == 1 && !_layoutInProgress)
            {
                _layoutInProgress = true;
                CalculateLayout(root, width, height, orientation, time);
                _layoutInProgress = false;
            }
            _applyDepth--;
        }

        public void ApplyComputedStyles(Rocket.Core.DOM.Node root, double time)
        {
            ApplyComputedStyles(root, ViewportWidth, ViewportHeight, Orientation, time);
        }

        public void Apply(Rocket.Core.DOM.Node root, int width, int height, Orientation orientation, double time)
        {
            CalculateLayout(root, width, height, orientation, time);
        }

        public void Apply(Rocket.Core.DOM.Node root, double time)
        {
            CalculateLayout(root, ViewportWidth, ViewportHeight, Orientation, time);
        }

        // Time-aware GenerateCss aliases (transitions do not affect raw stylesheet output).
        public string GenerateCss(int width, int height, Orientation orientation, double time)
        {
            CurrentTime = time;
            return BuildCss(width, height, orientation);
        }
        public string GenerateCss(double time)
        {
            CurrentTime = time;
            return BuildCss(ViewportWidth, ViewportHeight, Orientation);
        }

    }
}

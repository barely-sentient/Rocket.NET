using System;
using Rocket.Core.DOM;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

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

        private readonly ConcurrentDictionary<string, List<CssRule>> _ruleMatchCache =
            new ConcurrentDictionary<string, List<CssRule>>();

        private List<CssRule> GetMatchingRules(string tag, string id, string className, Node node, int width, int height, Orientation orientation)
        {
            string pseudoKey = node == null
                ? string.Empty
                : $"{node.Hovered}:{node.Focused}:{node.Active}:{node.Visited}:{node.Checked}:{node.Disabled}:{node.PseudoState}";
            string key = $"{tag}|{id}|{className}|{width}|{height}|{orientation}|{pseudoKey}|{Stylesheets.Count}|{_stylesheetVersion}";

            return _ruleMatchCache.GetOrAdd(key, _ => BuildMatchingRules(tag, id, className, node, width, height, orientation));
        }

        private List<CssRule> BuildMatchingRules(string tag, string id, string className, Node node, int width, int height, Orientation orientation)
        {
            var list = new List<CssRule>();
            foreach (var sheet in Stylesheets)
            {
                foreach (var rule in sheet.Rules)
                {
                    if (!MatchesMedia(rule.Media, width, height, orientation)) continue;
                    if (!SelectorMatches(rule.Selector, tag, id, className, node, this)) continue;
                    list.Add(rule);
                }
            }
            return list;
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

            string pseudoKey = $"{tag}|{id}|{className}|{node.Hovered}:{node.Focused}:{node.Active}:{node.Visited}:{node.Checked}:{node.Disabled}:{node.PseudoState}";
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
            var transitionSpecs = new List<CssTransition>();

            var matchingRules = GetMatchingRules(tag, id, className, node, width, height, orientation);
            foreach (var rule in matchingRules)
            {
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

            // Also collect transition specs from the merged target (in case transition declared as a normal decl).
            var mergedSpecs = ParseTransitions(target);
            if (mergedSpecs.Count > 0) transitionSpecs = mergedSpecs;

            // Manage transition state.
            var st = GetTransitionState(node);
            if (st == null) 
            {
                // No state tracking possible; just return target string.
                var r = DictToString(target);
                node._styleCacheW = width;
                node._styleCacheH = height;
                node._styleCacheO = orientation;
                node._styleCacheTime = time;
                node._styleCachePseudoKey = pseudoKey;
                node._styleCacheResult = r;
                node._styleCacheSheetVersion = _stylesheetVersion;
                node._styleCacheValid = true;
                return r;
            }

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

        public string ComputeStyle(Rocket.Core.DOM.Node node, int width, int height, Orientation orientation, TimeSpan time)
        {
            return ComputeStyle(node, width, height, orientation, time.TotalSeconds);
        }

        public string ComputeStyle(Rocket.Core.DOM.Node node, double time)
        {
            return ComputeStyle(node, ViewportWidth, ViewportHeight, Orientation, time);
        }

        public string ComputeStyle(Rocket.Core.DOM.Node node, TimeSpan time)
        {
            return ComputeStyle(node, ViewportWidth, ViewportHeight, Orientation, time.TotalSeconds);
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

        public string ComputeStyle(Rocket.Core.DOM.Node node, int width, int height, Orientation orientation, int timeMs)
        {
            return ComputeStyle(node, width, height, orientation, timeMs / 1000.0);
        }

        // Time-aware ComputeStyles
        public Dictionary<string, string> ComputeStyles(Rocket.Core.DOM.Node node, int width, int height, Orientation orientation, double time)
        {
            var style = ComputeStyle(node, width, height, orientation, time);
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var part in style.Split(';'))
            {
                var trimmed = part.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;
                var idx = trimmed.IndexOf(':');
                if (idx > 0)
                {
                    dict[trimmed.Substring(0, idx).Trim()] = trimmed.Substring(idx + 1).Trim();
                }
            }
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
            root.ComputedCss = style;
            if (root.ComputedStyles == null) root.ComputedStyles = new Dictionary<string, string>();
            root.ComputedStyles.Clear();
            foreach (var part in style.Split(';'))
            {
                var trimmed = part.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;
                var idx = trimmed.IndexOf(':');
                if (idx > 0)
                {
                    var key = trimmed.Substring(0, idx).Trim();
                    var val = trimmed.Substring(idx + 1).Trim();
                    root.ComputedStyles[key] = val;
                }
            }
            foreach (var child in root.Children ?? Array.Empty<Rocket.Core.DOM.Node>())
                ApplyComputedStyles(child, width, height, orientation, time);
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
        public string GenerateCss(int width, int height, Orientation orientation, TimeSpan time)
        {
            return GenerateCss(width, height, orientation, time.TotalSeconds);
        }
        public string Generate(int width, int height, Orientation orientation, double time) => GenerateCss(width, height, orientation, time);
        public string Generate(double time) => GenerateCss(time);
        public string GetCss(int width, int height, Orientation orientation, double time) => GenerateCss(width, height, orientation, time);
        public string GetCss(double time) => GenerateCss(time);
        public string BuildCss(int width, int height, Orientation orientation, double time) => GenerateCss(width, height, orientation, time);
        public string BuildCss(double time) => GenerateCss(time);

        private static string DictToString(Dictionary<string, string> dict)
        {
            var sb = new StringBuilder();
            foreach (var kv in dict)
            {
                sb.Append(kv.Key);
                sb.Append(": ");
                sb.Append(kv.Value);
                sb.Append("; ");
            }
            return sb.ToString().Trim();
        }
    }
}

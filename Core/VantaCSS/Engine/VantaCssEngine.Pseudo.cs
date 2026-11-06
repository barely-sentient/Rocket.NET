using System;
using Rocket.Core.DOM;
using System.Collections.Generic;

namespace VantaCSS
{
    /// <summary>
    /// Pseudo-class (:hover, :focus, ...) support for <see cref="VantaCssEngine"/>.
    /// </summary>
    /// <remarks>
    /// A pseudo-class describes an interactive state of a node. The engine
    /// keeps track of which node is hovered/focused/active and exposes
    /// helpers so that CSS rules such as <c>button:hover</c> only contribute
    /// to the computed style when their pseudo-class matches.
    /// </remarks>
    public partial class VantaCssEngine
    {
        // -----------------------------------------------------------------
        // Engine-level interaction state
        // -----------------------------------------------------------------

        /// <summary>Optional global pseudo-state applied to all nodes (e.g. "hover").</summary>
        public string PseudoState { get; set; }
        /// <summary>Optional global pseudo-states applied to all nodes.</summary>
        public HashSet<string> PseudoStates { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Sets a global pseudo-state.</summary>
        public void SetPseudoState(string state)
        {
            if (string.IsNullOrWhiteSpace(state)) return;
            PseudoState = state.Trim();
            PseudoStates.Add(PseudoState);
        }

        /// <summary>Clears global pseudo-state.</summary>
        public void ClearPseudoState()
        {
            PseudoState = null;
            PseudoStates.Clear();
        }

        /// <summary>The node currently considered hovered.</summary>
        public Node HoveredNode { get; set; }
        /// <summary>The node currently considered focused.</summary>
        public Node FocusedNode { get; set; }
        /// <summary>The node currently considered active.</summary>
        public Node ActiveNode { get; set; }

        /// <summary>Nodes currently considered hovered (multi).</summary>
        public HashSet<Node> HoveredNodes { get; } = new HashSet<Node>();
        /// <summary>Nodes currently considered focused (multi).</summary>
        public HashSet<Node> FocusedNodes { get; } = new HashSet<Node>();
        /// <summary>Nodes currently considered active (multi).</summary>
        public HashSet<Node> ActiveNodes { get; } = new HashSet<Node>();

        /// <summary>Marks <paramref name="node"/> as hovered.</summary>
        public void Hover(Node node, bool hovered = true)
        {
            if (node == null) return;
            if (!hovered)
            {
                node.Hovered = false;
                HoveredNodes.Remove(node);
                if (HoveredNode == node) HoveredNode = null;
                return;
            }
            ClearHover();
            HoveredNode = node;
            node.Hovered = true;
            HoveredNodes.Add(node);
        }

        /// <summary>Clears hover state.</summary>
        public void ClearHover()
        {
            if (HoveredNode != null) HoveredNode.Hovered = false;
            foreach (var n in HoveredNodes) if (n != null) n.Hovered = false;
            HoveredNode = null;
            HoveredNodes.Clear();
        }

        /// <summary>Marks <paramref name="node"/> as focused.</summary>
        public void Focus(Node node, bool focused = true)
        {
            if (node == null) return;
            if (!focused)
            {
                node.Focused = false;
                FocusedNodes.Remove(node);
                if (FocusedNode == node) FocusedNode = null;
                return;
            }
            ClearFocus();
            FocusedNode = node;
            node.Focused = true;
            FocusedNodes.Add(node);
        }

        /// <summary>Clears focus state.</summary>
        public void ClearFocus()
        {
            if (FocusedNode != null) FocusedNode.Focused = false;
            foreach (var n in FocusedNodes) if (n != null) n.Focused = false;
            FocusedNode = null;
            FocusedNodes.Clear();
        }

        /// <summary>Marks <paramref name="node"/> as active.</summary>
        public void Activate(Node node, bool active = true)
        {
            if (node == null) return;
            if (!active)
            {
                node.Active = false;
                ActiveNodes.Remove(node);
                if (ActiveNode == node) ActiveNode = null;
                return;
            }
            ClearActive();
            ActiveNode = node;
            node.Active = true;
            ActiveNodes.Add(node);
        }

        /// <summary>Clears active state.</summary>
        public void ClearActive()
        {
            if (ActiveNode != null) ActiveNode.Active = false;
            foreach (var n in ActiveNodes) if (n != null) n.Active = false;
            ActiveNode = null;
            ActiveNodes.Clear();
        }

        /// <summary>Sets a pseudo-state string on a node.</summary>
        public void SetPseudoState(Node node, string state)
        {
            if (node == null) return;
            var clean = (state ?? string.Empty).Trim().TrimStart(':');
            node.PseudoState = clean;
            if (string.Equals(clean, "hover", StringComparison.OrdinalIgnoreCase)) node.Hovered = true;
            if (string.Equals(clean, "focus", StringComparison.OrdinalIgnoreCase)) node.Focused = true;
            if (string.Equals(clean, "active", StringComparison.OrdinalIgnoreCase)) node.Active = true;
        }

        /// <summary>Convenience: set hover/focus/active flags directly on a node.</summary>
        public void SetState(Node node, bool hover = false, bool focus = false, bool active = false, bool visited = false, bool checkedState = false, bool disabled = false)
        {
            if (node == null) return;
            node.Hovered = hover;
            node.Focused = focus;
            node.Active = active;
            node.Visited = visited;
            node.Checked = checkedState;
            node.Disabled = disabled;
            if (hover) { HoveredNode = node; HoveredNodes.Add(node); }
            if (focus) { FocusedNode = node; FocusedNodes.Add(node); }
            if (active) { ActiveNode = node; ActiveNodes.Add(node); }
        }


        /// <summary>Whether the given node is currently hovered.</summary>
        public bool IsHovered(Node node)
        {
            if (node == null) return false;
            if (node.Hovered || HoveredNode == node || HoveredNodes.Contains(node)) return true;
            if (!string.IsNullOrWhiteSpace(PseudoState) && PseudoState.Trim().TrimStart(':').Equals("hover", StringComparison.OrdinalIgnoreCase)) return true;
            if (PseudoStates.Contains("hover") || PseudoStates.Contains(":hover")) return true;
            return false;
        }

        /// <summary>Whether the given node is currently focused.</summary>
        public bool IsFocused(Node node)
        {
            if (node == null) return false;
            if (node.Focused || FocusedNode == node || FocusedNodes.Contains(node)) return true;
            if (!string.IsNullOrWhiteSpace(PseudoState) && PseudoState.Trim().TrimStart(':').Equals("focus", StringComparison.OrdinalIgnoreCase)) return true;
            if (PseudoStates.Contains("focus") || PseudoStates.Contains(":focus")) return true;
            return false;
        }

        /// <summary>Whether the given node is currently active.</summary>
        public bool IsActive(Node node)
        {
            if (node == null) return false;
            if (node.Active || ActiveNode == node || ActiveNodes.Contains(node)) return true;
            if (!string.IsNullOrWhiteSpace(PseudoState) && PseudoState.Trim().TrimStart(':').Equals("active", StringComparison.OrdinalIgnoreCase)) return true;
            if (PseudoStates.Contains("active") || PseudoStates.Contains(":active")) return true;
            return false;
        }

        /// <summary>Whether the given pseudo-class matches the node in the current engine state.</summary>
        public bool MatchesPseudo(Node node, string pseudo)
        {
            if (node == null) return false;
            // Engine-level state takes precedence and keeps node flags in sync.
            string p = (pseudo ?? string.Empty).Trim().TrimStart(':').ToLowerInvariant();
            switch (p)
            {
                case "hover": return IsHovered(node);
                case "focus": return IsFocused(node);
                case "active": return IsActive(node);
                default: return node.MatchesPseudo(pseudo);
            }
        }
    }
}

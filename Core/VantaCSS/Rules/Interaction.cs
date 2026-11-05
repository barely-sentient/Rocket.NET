namespace VantaCSS.Engine
{
    public partial class NodeComputedRuleset
    {
        /// <summary>
        /// Gets or sets the mouse cursor displayed when hovering over the element.
        /// </summary>
        public Cursor Cursor = Cursor.Auto;

        /// <summary>
        /// Gets or sets whether the element responds to pointer events.
        /// </summary>
        public PointerEvents PointerEvents = PointerEvents.Auto;

        /// <summary>
        /// Gets or sets whether the user can select text within the element.
        /// </summary>
        public UserSelect UserSelect = UserSelect.Auto;

        /// <summary>
        /// Gets or sets whether and how the element can be resized by the user.
        /// </summary>
        public Resize Resize = Resize.None;

        /// <summary>
        /// Gets or sets the touch action behavior.
        /// </summary>
        public TouchAction TouchAction = TouchAction.Auto;

        /// <summary>
        /// Gets or sets the scroll behavior.
        /// </summary>
        public ScrollBehavior ScrollBehavior = ScrollBehavior.Auto;

        /// <summary>
        /// Gets or sets the overscroll behavior on both axes.
        /// </summary>
        public OverscrollBehavior OverscrollBehavior = OverscrollBehavior.Auto;
    }

    /// <summary>
    /// Specifies the mouse cursor type.
    /// </summary>
    public enum Cursor : byte
    {
        Auto, Default, None, ContextMenu, Help, Pointer, Progress, Wait, Cell, Crosshair,
        Text, VerticalText, Alias, Copy, Move, NoDrop, NotAllowed, Grab, Grabbing,
        AllScroll, ColResize, RowResize, NResize, EResize, SResize, WResize,
        NeResize, NwResize, SeResize, SwResize, EwResize, NsResize, NeswResize, NwseResize,
        ZoomIn, ZoomOut,
        Initial, Inherit, Unset
    }

    /// <summary>
    /// Specifies whether an element responds to pointer events.
    /// </summary>
    public enum PointerEvents : byte
    {
        Auto, None, VisiblePainted, VisibleFill, VisibleStroke, Visible, Painted, Fill, Stroke, All,
        Initial, Inherit, Unset
    }

    /// <summary>
    /// Specifies whether text can be selected.
    /// </summary>
    public enum UserSelect : byte
    {
        Auto, None, Text, All, Contain,
        Initial, Inherit, Unset
    }

    /// <summary>
    /// Specifies whether an element is resizable by the user.
    /// </summary>
    public enum Resize : byte
    {
        None, Both, Horizontal, Vertical, Block, Inline,
        Initial, Inherit, Unset
    }

    /// <summary>
    /// Specifies the touch action behavior.
    /// </summary>
    public enum TouchAction : byte
    {
        Auto, None, PanX, PanY, PanLeft, PanRight, PanUp, PanDown, PinchZoom, Manipulation,
        Initial, Inherit, Unset
    }

    /// <summary>
    /// Specifies the scroll behavior.
    /// </summary>
    public enum ScrollBehavior : byte
    {
        Auto, Smooth,
        Initial, Inherit, Unset
    }

    /// <summary>
    /// Specifies the overscroll behavior.
    /// </summary>
    public enum OverscrollBehavior : byte
    {
        Auto, Contain, None,
        Initial, Inherit, Unset
    }
}
using VantaCSS.Rules;

namespace VantaCSS.Engine
{
    public partial class NodeComputedRuleset
    {
        /// <summary>
        /// Added the display property
        /// </summary>
        public DisplayMode Display = DisplayMode.Inherit;
    }

    /// <summary>
    /// Specifies how an element is displayed and how its child elements participate
    /// in the layout flow.
    /// </summary>
    public enum DisplayMode
    {
        /// <summary>
        /// Inherits the display value from its parent element.
        /// </summary>
        Inherit,

        /// <summary>
        /// Displays the element as an inline element within the current text flow.
        /// </summary>
        Inline,

        /// <summary>
        /// Displays the element as a block-level element.
        /// </summary>
        Block,

        /// <summary>
        /// Displays the element as an inline-level block container.
        /// </summary>
        InlineBlock,

        /// <summary>
        /// Displays the element as an inline-level grid container.
        /// </summary>
        InlineGrid,

        /// <summary>
        /// Displays the element as a grid container.
        /// </summary>
        Grid,

        /// <summary>
        /// Displays the element as a flex container.
        /// </summary>
        Flex,

        /// <summary>
        /// Displays the element as an inline-level flex container.
        /// </summary>
        InlineFlex,

        /// <summary>
        /// Displays the element as a flow-root block container, establishing a new block formatting context.
        /// </summary>
        FlowRoot,

        /// <summary>
        /// Displays the element as a list item.
        /// </summary>
        ListItem,

        /// <summary>
        /// Displays the element as a table.
        /// </summary>
        Table,

        /// <summary>
        /// Displays the element as an inline-level table.
        /// </summary>
        InlineTable,

        /// <summary>
        /// Displays the element as a table row group.
        /// </summary>
        TableRowGroup,

        /// <summary>
        /// Displays the element as a table header group.
        /// </summary>
        TableHeaderGroup,

        /// <summary>
        /// Displays the element as a table footer group.
        /// </summary>
        TableFooterGroup,

        /// <summary>
        /// Displays the element as a table row.
        /// </summary>
        TableRow,

        /// <summary>
        /// Displays the element as a table cell.
        /// </summary>
        TableCell,

        /// <summary>
        /// Displays the element as a table column group.
        /// </summary>
        TableColumnGroup,

        /// <summary>
        /// Displays the element as a table column.
        /// </summary>
        TableColumn,

        /// <summary>
        /// Displays the element as a table caption.
        /// </summary>
        TableCaption,

        /// <summary>
        /// Displays the element as a ruby container.
        /// </summary>
        Ruby,

        /// <summary>
        /// Displays the element as an inline-level ruby container.
        /// </summary>
        RubyInline,

        /// <summary>
        /// Displays the element as a ruby annotation container.
        /// </summary>
        RubyBase,

        /// <summary>
        /// Displays the element as a ruby annotation container.
        /// </summary>
        RubyText,

        /// <summary>
        /// Displays the element as a ruby base container.
        /// </summary>
        RubyBaseContainer,

        /// <summary>
        /// Displays the element as a ruby text container.
        /// </summary>
        RubyTextContainer,

        /// <summary>
        /// Removes the element and its descendants from the layout tree.
        /// </summary>
        None,

        /// <summary>
        /// Hides the element while preserving its layout space.
        /// </summary>
        Contents,

        /// <summary>
        /// Uses the browser's default display behavior for the element.
        /// </summary>
        Revert,

        /// <summary>
        /// Reverts the display property to the browser's user-agent stylesheet value.
        /// </summary>
        RevertLayer,

        /// <summary>
        /// Resets the display property to its initial CSS value.
        /// </summary>
        Initial,

        /// <summary>
        /// Uses the display value inherited from the parent element.
        /// </summary>
        Unset
    }
}
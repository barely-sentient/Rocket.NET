namespace VantaCSS.Engine
{
    public partial class NodeComputedRuleset
    {
        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        public string BackgroundColor { get; set; } = "";

        /// <summary>
        /// Gets or sets the background image URL or gradient.
        /// </summary>
        public string BackgroundImage { get; set; } = "";

        /// <summary>
        /// Gets or sets the background repeat behavior.
        /// </summary>
        public BackgroundRepeat BackgroundRepeat = BackgroundRepeat.Repeat;

        /// <summary>
        /// Gets or sets the background position.
        /// </summary>
        public string BackgroundPosition { get; set; } = "";

        /// <summary>
        /// Gets or sets the background size.
        /// </summary>
        public string BackgroundSize { get; set; } = "";

        /// <summary>
        /// Gets or sets the background attachment.
        /// </summary>
        public BackgroundAttachment BackgroundAttachment = BackgroundAttachment.Scroll;

        /// <summary>
        /// Gets or sets the background clip.
        /// </summary>
        public BackgroundClip BackgroundClip = BackgroundClip.BorderBox;

        /// <summary>
        /// Gets or sets the background origin.
        /// </summary>
        public BackgroundOrigin BackgroundOrigin = BackgroundOrigin.PaddingBox;

        /// <summary>
        /// Gets or sets the background blend mode.
        /// </summary>
        public BlendMode BackgroundBlendMode = BlendMode.Normal;

        /// <summary>
        /// Gets or sets the text color.
        /// </summary>
        public string Color { get; set; } = "";

        /// <summary>
        /// Gets or sets how the replaced element's content is resized to fit its container.
        /// </summary>
        public ObjectFit ObjectFit = ObjectFit.Fill;

        /// <summary>
        /// Gets or sets the alignment of the replaced element's content within its box.
        /// </summary>
        public string ObjectPosition { get; set; } = "50% 50%";
    }

    /// <summary>
    /// Specifies how a background image is repeated.
    /// </summary>
    public enum BackgroundRepeat : byte
    {
        /// <summary>
        /// The image is repeated both horizontally and vertically.
        /// </summary>
        Repeat,

        /// <summary>
        /// The image is repeated horizontally only.
        /// </summary>
        RepeatX,

        /// <summary>
        /// The image is repeated vertically only.
        /// </summary>
        RepeatY,

        /// <summary>
        /// The image is not repeated.
        /// </summary>
        NoRepeat,

        /// <summary>
        /// The image is repeated and clipped to fit.
        /// </summary>
        Space,

        /// <summary>
        /// The image is repeated and resized to fit.
        /// </summary>
        Round,

        /// <summary>
        /// Resets to the initial CSS value.
        /// </summary>
        Initial,

        /// <summary>
        /// Inherits the value from the parent element.
        /// </summary>
        Inherit,

        /// <summary>
        /// Uses the inherited value if the property is inherited, otherwise the initial value.
        /// </summary>
        Unset
    }

    /// <summary>
    /// Specifies whether a background image scrolls with the content.
    /// </summary>
    public enum BackgroundAttachment : byte
    {
        /// <summary>
        /// The background scrolls with the content.
        /// </summary>
        Scroll,

        /// <summary>
        /// The background is fixed relative to the viewport.
        /// </summary>
        Fixed,

        /// <summary>
        /// The background scrolls with the element's contents.
        /// </summary>
        Local,

        /// <summary>
        /// Resets to the initial CSS value.
        /// </summary>
        Initial,

        /// <summary>
        /// Inherits the value from the parent element.
        /// </summary>
        Inherit,

        /// <summary>
        /// Uses the inherited value if the property is inherited, otherwise the initial value.
        /// </summary>
        Unset
    }

    /// <summary>
    /// Specifies the painting area of the background.
    /// </summary>
    public enum BackgroundClip : byte
    {
        /// <summary>
        /// Background extends to the border box.
        /// </summary>
        BorderBox,

        /// <summary>
        /// Background extends to the padding box.
        /// </summary>
        PaddingBox,

        /// <summary>
        /// Background extends to the content box.
        /// </summary>
        ContentBox,

        /// <summary>
        /// Background is clipped to the text.
        /// </summary>
        Text,

        /// <summary>
        /// Resets to the initial CSS value.
        /// </summary>
        Initial,

        /// <summary>
        /// Inherits the value from the parent element.
        /// </summary>
        Inherit,

        /// <summary>
        /// Uses the inherited value if the property is inherited, otherwise the initial value.
        /// </summary>
        Unset
    }

    /// <summary>
    /// Specifies the positioning area of the background.
    /// </summary>
    public enum BackgroundOrigin : byte
    {
        /// <summary>
        /// Position relative to the border box.
        /// </summary>
        BorderBox,

        /// <summary>
        /// Position relative to the padding box.
        /// </summary>
        PaddingBox,

        /// <summary>
        /// Position relative to the content box.
        /// </summary>
        ContentBox,

        /// <summary>
        /// Resets to the initial CSS value.
        /// </summary>
        Initial,

        /// <summary>
        /// Inherits the value from the parent element.
        /// </summary>
        Inherit,

        /// <summary>
        /// Uses the inherited value if the property is inherited, otherwise the initial value.
        /// </summary>
        Unset
    }

    /// <summary>
    /// Specifies a blend mode.
    /// </summary>
    public enum BlendMode : byte
    {
        /// <summary>
        /// No blending; normal rendering.
        /// </summary>
        Normal,

        /// <summary>
        /// Multiplies source and backdrop.
        /// </summary>
        Multiply,

        /// <summary>
        /// Screens source and backdrop.
        /// </summary>
        Screen,

        /// <summary>
        /// Overlays source on backdrop.
        /// </summary>
        Overlay,

        /// <summary>
        /// Darkens the backdrop.
        /// </summary>
        Darken,

        /// <summary>
        /// Lightens the backdrop.
        /// </summary>
        Lighten,

        /// <summary>
        /// Color dodge blend.
        /// </summary>
        ColorDodge,

        /// <summary>
        /// Color burn blend.
        /// </summary>
        ColorBurn,

        /// <summary>
        /// Hard light blend.
        /// </summary>
        HardLight,

        /// <summary>
        /// Soft light blend.
        /// </summary>
        SoftLight,

        /// <summary>
        /// Difference blend.
        /// </summary>
        Difference,

        /// <summary>
        /// Exclusion blend.
        /// </summary>
        Exclusion,

        /// <summary>
        /// Hue blend.
        /// </summary>
        Hue,

        /// <summary>
        /// Saturation blend.
        /// </summary>
        Saturation,

        /// <summary>
        /// Color blend.
        /// </summary>
        Color,

        /// <summary>
        /// Luminosity blend.
        /// </summary>
        Luminosity,

        /// <summary>
        /// Resets to the initial CSS value.
        /// </summary>
        Initial,

        /// <summary>
        /// Inherits the value from the parent element.
        /// </summary>
        Inherit,

        /// <summary>
        /// Uses the inherited value if the property is inherited, otherwise the initial value.
        /// </summary>
        Unset
    }

    /// <summary>
    /// Specifies how a replaced element's content is resized to fit its container.
    /// </summary>
    public enum ObjectFit : byte
    {
        /// <summary>
        /// The content is stretched to fill the container.
        /// </summary>
        Fill,

        /// <summary>
        /// The content is scaled to maintain aspect ratio while fitting within the container.
        /// </summary>
        Contain,

        /// <summary>
        /// The content is scaled to maintain aspect ratio while filling the container (may crop).
        /// </summary>
        Cover,

        /// <summary>
        /// The content is not resized.
        /// </summary>
        None,

        /// <summary>
        /// The content is scaled down to fit if needed.
        /// </summary>
        ScaleDown,

        /// <summary>
        /// Resets to the initial CSS value.
        /// </summary>
        Initial,

        /// <summary>
        /// Inherits the value from the parent element.
        /// </summary>
        Inherit,

        /// <summary>
        /// Uses the inherited value if the property is inherited, otherwise the initial value.
        /// </summary>
        Unset
    }
}
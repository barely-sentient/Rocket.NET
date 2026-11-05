namespace VantaCSS.Tokeniser
{
    /// <summary>
    /// Represents a token within a parsed document.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// What kind of token is this
        /// </summary>
        public TokenKind Kind { get; }

        /// <summary>
        /// The starting position within the source
        /// </summary>
        public uint StartIndex { get; }

        /// <summary>
        /// The length of the value.
        /// </summary>
        public uint Length { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="Token"/> class.
        /// </summary>
        public Token(TokenKind kind, uint startIndex, uint length)
        {
            Kind = kind;
            StartIndex = startIndex;
            Length = length;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Token"/> class.
        /// </summary>
        public Token(TokenKind kind, int startIndex, int length)
            : this(kind, (uint)startIndex, (uint)length)
        {
        }
    }
}
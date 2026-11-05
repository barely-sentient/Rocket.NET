using System;
using System.Collections.Generic;
using System.Text;

namespace VantaCSS.Tokeniser
{
    /// <summary>
    /// Provides CSS tokenisation.
    /// </summary>
    internal static class Tokeniser
    {
        private static readonly HashSet<string> ColorKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "black", "white", "red", "green", "blue", "yellow", "cyan", "magenta",
            "orange", "purple", "pink", "brown", "gray", "grey", "lime", "maroon",
            "navy", "olive", "silver", "teal", "aqua", "fuchsia", "transparent",
            "currentcolor", "inherit", "initial", "unset", "revert"
        };

        /// <summary>
        /// Parses the supplied CSS source into a list of tokens.
        /// </summary>
        /// <param name="css">The CSS source to tokenise.</param>
        /// <returns>A list of tokens.</returns>
        internal static List<Token> Parse(string css)
        {
            var tokens = new List<Token>();
            if (css == null)
            {
                return tokens;
            }

            int i = 0;
            int n = css.Length;
            int depth = 0;
            bool expectProperty = true;

            while (i < n)
            {
                char c = css[i];

                // Whitespace
                if (char.IsWhiteSpace(c))
                {
                    int start = i;
                    while (i < n && char.IsWhiteSpace(css[i]))
                    {
                        i++;
                    }
                    tokens.Add(new Token(TokenKind.Whitespace, start, i - start));
                    continue;
                }

                // Comments /* ... */
                if (i + 1 < n && c == '/' && css[i + 1] == '*')
                {
                    int start = i;
                    i += 2;
                    while (i + 1 < n && !(css[i] == '*' && css[i + 1] == '/'))
                    {
                        i++;
                    }
                    if (i + 1 < n)
                    {
                        i += 2;
                    }
                    else
                    {
                        i = n;
                    }
                    tokens.Add(new Token(TokenKind.Comment, start, i - start));
                    continue;
                }

                // Line comments // ... (not standard CSS but harmless)
                if (i + 1 < n && c == '/' && css[i + 1] == '/')
                {
                    int start = i;
                    while (i < n && css[i] != '\n')
                    {
                        i++;
                    }
                    tokens.Add(new Token(TokenKind.Comment, start, i - start));
                    continue;
                }

                // CDO / CDC
                if (i + 3 < n && c == '<' && css[i + 1] == '!' && css[i + 2] == '-' && css[i + 3] == '-')
                {
                    tokens.Add(new Token(TokenKind.Cdo, i, 4));
                    i += 4;
                    continue;
                }
                if (i + 2 < n && c == '-' && css[i + 1] == '-' && css[i + 2] == '>')
                {
                    tokens.Add(new Token(TokenKind.Cdc, i, 3));
                    i += 3;
                    continue;
                }

                // Strings
                if (c == '"' || c == '\'')
                {
                    int start = i;
                    char quote = c;
                    i++;
                    while (i < n)
                    {
                        if (css[i] == '\\')
                        {
                            i += 2;
                            if (i > n)
                            {
                                i = n;
                            }
                            continue;
                        }
                        if (css[i] == quote)
                        {
                            i++;
                            break;
                        }
                        i++;
                    }
                    tokens.Add(new Token(TokenKind.String, start, i - start));
                    continue;
                }

                // At-keyword / directive
                if (c == '@')
                {
                    int start = i;
                    i++; // skip @
                    ReadIdent(css, ref i);
                    tokens.Add(new Token(TokenKind.AtKeyword, start, i - start));
                    // trim trailing whitespace already handled next loop
                    continue;
                }

                // Class name
                if (c == '.' && i + 1 < n && IsIdentStart(css[i + 1]))
                {
                    int start = i;
                    i++; // skip .
                    ReadIdent(css, ref i);
                    tokens.Add(new Token(TokenKind.ClassName, start, i - start));
                    continue;
                }

                // ID selector or hex color
                if (c == '#')
                {
                    int start = i;
                    // try hex color
                    int j = i + 1;
                    int hexCount = 0;
                    while (j < n && IsHexDigit(css[j]) && hexCount < 8)
                    {
                        j++;
                        hexCount++;
                    }
                    if (hexCount == 3 || hexCount == 4 || hexCount == 6 || hexCount == 8)
                    {
                        // ensure not followed by ident char that would make it an id
                        if (j >= n || !IsIdentChar(css[j]))
                        {
                            tokens.Add(new Token(TokenKind.HexColor, start, j - start));
                            i = j;
                            continue;
                        }
                    }
                    // otherwise ID selector
                    i++; // skip #
                    if (i < n && IsIdentStart(css[i]))
                    {
                        ReadIdent(css, ref i);
                    }
                    tokens.Add(new Token(TokenKind.Identifier, start, i - start));
                    continue;
                }

                // Universal selector / asterisk
                if (c == '*')
                {
                    tokens.Add(new Token(TokenKind.UniversalSelector, i, 1));
                    i++;
                    continue;
                }

                // Combinators and delimiters
                if (c == '>')
                {
                    tokens.Add(new Token(TokenKind.GreaterThan, i, 1));
                    i++;
                    continue;
                }
                if (c == '+')
                {
                    tokens.Add(new Token(TokenKind.Plus, i, 1));
                    i++;
                    continue;
                }
                if (c == '~')
                {
                    tokens.Add(new Token(TokenKind.Tilde, i, 1));
                    i++;
                    continue;
                }
                if (c == '&')
                {
                    tokens.Add(new Token(TokenKind.Ampersand, i, 1));
                    i++;
                    continue;
                }
                if (c == ',')
                {
                    tokens.Add(new Token(TokenKind.Comma, i, 1));
                    i++;
                    continue;
                }
                if (c == ';')
                {
                    tokens.Add(new Token(TokenKind.Semicolon, i, 1));
                    i++;
                    expectProperty = true;
                    continue;
                }
                if (c == '{')
                {
                    tokens.Add(new Token(TokenKind.OpenBrace, i, 1));
                    i++;
                    depth++;
                    expectProperty = true;
                    continue;
                }
                if (c == '}')
                {
                    tokens.Add(new Token(TokenKind.CloseBrace, i, 1));
                    i++;
                    if (depth > 0)
                    {
                        depth--;
                    }
                    expectProperty = true;
                    continue;
                }
                if (c == '(')
                {
                    tokens.Add(new Token(TokenKind.OpenParen, i, 1));
                    i++;
                    continue;
                }
                if (c == ')')
                {
                    tokens.Add(new Token(TokenKind.CloseParen, i, 1));
                    i++;
                    continue;
                }
                if (c == '[')
                {
                    tokens.Add(new Token(TokenKind.OpenBracket, i, 1));
                    i++;
                    continue;
                }
                if (c == ']')
                {
                    tokens.Add(new Token(TokenKind.CloseBracket, i, 1));
                    i++;
                    continue;
                }

                // Colon / double colon
                if (c == ':')
                {
                    if (i + 1 < n && css[i + 1] == ':')
                    {
                        tokens.Add(new Token(TokenKind.DoubleColon, i, 2));
                        i += 2;
                    }
                    else
                    {
                        tokens.Add(new Token(TokenKind.Colon, i, 1));
                        i++;
                    }
                    expectProperty = false;
                    continue;
                }

                // !important
                if (c == '!')
                {
                    // read following identifier
                    int start = i;
                    i++;
                    // skip whitespace
                    while (i < n && char.IsWhiteSpace(css[i]))
                    {
                        i++;
                    }
                    int identStart = i;
                    ReadIdent(css, ref i);
                    string ident = css.Substring(identStart, i - identStart);
                    if (string.Equals(ident, "important", StringComparison.OrdinalIgnoreCase))
                    {
                        tokens.Add(new Token(TokenKind.Important, start, i - start));
                    }
                    else
                    {
                        tokens.Add(new Token(TokenKind.Delimiter, start, 1));
                        i = start + 1;
                    }
                    continue;
                }

                // Numbers / dimensions
                if (char.IsDigit(c) || (c == '.' && i + 1 < n && char.IsDigit(css[i + 1])) ||
                    ((c == '+' || c == '-') && i + 1 < n && char.IsDigit(css[i + 1])))
                {
                    int start = i;
                    // optional sign
                    if (i < n && (css[i] == '+' || css[i] == '-'))
                    {
                        i++;
                    }
                    // integer part
                    while (i < n && char.IsDigit(css[i]))
                    {
                        i++;
                    }
                    // fractional
                    if (i < n && css[i] == '.')
                    {
                        i++;
                        while (i < n && char.IsDigit(css[i]))
                        {
                            i++;
                        }
                    }
                    // exponent
                    if (i < n && (css[i] == 'e' || css[i] == 'E'))
                    {
                        int save = i;
                        i++;
                        if (i < n && (css[i] == '+' || css[i] == '-'))
                        {
                            i++;
                        }
                        if (i < n && char.IsDigit(css[i]))
                        {
                            while (i < n && char.IsDigit(css[i]))
                            {
                                i++;
                            }
                        }
                        else
                        {
                            i = save;
                        }
                    }
                    bool isFloat = css.Substring(start, i - start).IndexOf('.') >= 0 ||
                                   css.Substring(start, i - start).IndexOf('e') >= 0 ||
                                   css.Substring(start, i - start).IndexOf('E') >= 0;

                    // unit
                    int unitStart = i;
                    if (i < n && css[i] == '%')
                    {
                        i++;
                    }
                    else if (i < n && IsIdentStart(css[i]))
                    {
                        ReadIdent(css, ref i);
                    }
                    string unit = css.Substring(unitStart, i - unitStart);

                    TokenKind kind;
                    if (string.IsNullOrEmpty(unit))
                    {
                        kind = isFloat ? TokenKind.Number : TokenKind.Integer;
                    }
                    else
                    {
                        kind = UnitTokenKind(unit);
                        if (kind == TokenKind.Unknown)
                        {
                            kind = TokenKind.Dimension;
                        }
                    }

                    tokens.Add(new Token(kind, start, i - start));
                    continue;
                }

                // Identifiers / property names / values / tags / functions / url()
                if (IsIdentStart(c))
                {
                    int start = i;
                    ReadIdent(css, ref i);
                    string ident = css.Substring(start, i - start);

                    // url(...) special handling
                    if (string.Equals(ident, "url", StringComparison.OrdinalIgnoreCase))
                    {
                        // skip whitespace
                        int j = i;
                        while (j < n && char.IsWhiteSpace(css[j]))
                        {
                            j++;
                        }
                        if (j < n && css[j] == '(')
                        {
                            // consume until matching close paren
                            int depthParen = 0;
                            int k = i;
                            // include leading whitespace? We'll skip whitespace tokens? Simpler: consume whitespace into url token
                            while (k < n && char.IsWhiteSpace(css[k]))
                            {
                                k++;
                            }
                            // now k at '('
                            depthParen = 0;
                            while (k < n)
                            {
                                if (css[k] == '(')
                                {
                                    depthParen++;
                                    k++;
                                }
                                else if (css[k] == ')')
                                {
                                    depthParen--;
                                    k++;
                                    if (depthParen == 0)
                                    {
                                        break;
                                    }
                                }
                                else if (css[k] == '"' || css[k] == '\'')
                                {
                                    char q = css[k];
                                    k++;
                                    while (k < n && css[k] != q)
                                    {
                                        if (css[k] == '\\')
                                        {
                                            k++;
                                        }
                                        k++;
                                    }
                                    if (k < n)
                                    {
                                        k++;
                                    }
                                }
                                else
                                {
                                    k++;
                                }
                            }
                            tokens.Add(new Token(TokenKind.Url, start, k - start));
                            i = k;
                            continue;
                        }
                    }

                    // function handling: if next non-space is '('
                    int p = i;
                    while (p < n && char.IsWhiteSpace(css[p]))
                    {
                        p++;
                    }
                    bool isFunction = (p < n && css[p] == '(');
                    bool followedByColon = (p < n && css[p] == ':') && !(p + 1 < n && css[p + 1] == ':');

                    TokenKind kind;
                    if (ident.StartsWith("--", StringComparison.Ordinal))
                    {
                        kind = TokenKind.VariableName;
                    }
                    else if (isFunction)
                    {
                        kind = TokenKind.Function;
                    }
                    else if (followedByColon)
                    {
                        kind = TokenKind.RuleName;
                    }
                    else if (depth == 0)
                    {
                        // selector-level identifier
                        kind = TokenKind.TagName;
                    }
                    else
                    {
                        // inside braces
                        if (expectProperty)
                        {
                            kind = TokenKind.RuleName;
                        }
                        else
                        {
                            // value position
                            if (ColorKeywords.Contains(ident))
                            {
                                kind = TokenKind.Color;
                            }
                            else
                            {
                                kind = TokenKind.Keyword;
                            }
                        }
                    }

                    tokens.Add(new Token(kind, start, i - start));
                    // after reading a property name, expect colon next (but whitespace may follow)
                    if (depth > 0 && expectProperty && kind == TokenKind.RuleName)
                    {
                        // leave expectProperty true until colon
                    }
                    continue;
                }

                // Variables as property? handled by ident starts with -- above.

                // Fallback: single delimiter char
                tokens.Add(new Token(TokenKind.Delimiter, i, 1));
                i++;
            }

            tokens.Add(new Token(TokenKind.EndOfFile, n, 0));

            // Populate matched source text for every token.
            for (int t = 0; t < tokens.Count; t++)
            {
                var tok = tokens[t];
                int start = (int)tok.StartIndex;
                int len = (int)tok.Length;
                if (start + len <= css.Length)
                {
                    tokens[t] = new Token(tok.Kind, start, len, css.Substring(start, len));
                }
            }

            return tokens;
        }

        private static void ReadIdent(string css, ref int i)
        {
            int n = css.Length;
            // handle leading -- already part? ReadIdent called after @ or . etc where i at ident start
            while (i < n && IsIdentChar(css[i]))
            {
                i++;
            }
        }

        private static bool IsIdentStart(char c)
        {
            return char.IsLetter(c) || c == '_' || c == '-' || c == '\\' || (c >= 0x80);
        }

        private static bool IsIdentChar(char c)
        {
            return char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == '\\' || (c >= 0x80);
        }

        private static bool IsHexDigit(char c)
        {
            return (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
        }

        private static TokenKind UnitTokenKind(string unit)
        {
            switch (unit.ToLowerInvariant())
            {
                case "px": return TokenKind.PixelUnit;
                case "%": return TokenKind.PercentUnit;
                case "em": return TokenKind.EmUnit;
                case "rem": return TokenKind.RemUnit;
                case "vw": return TokenKind.ViewportWidthUnit;
                case "vh": return TokenKind.ViewportHeightUnit;
                case "pt": return TokenKind.PointUnit;
                case "pc": return TokenKind.PicaUnit;
                case "in": return TokenKind.InchUnit;
                case "cm": return TokenKind.CentimeterUnit;
                case "mm": return TokenKind.MillimeterUnit;
                case "q": return TokenKind.QuarterMillimeterUnit;
                case "ex": return TokenKind.ExUnit;
                case "ch": return TokenKind.ChUnit;
                case "vmin": return TokenKind.ViewportMinUnit;
                case "vmax": return TokenKind.ViewportMaxUnit;
                case "fr": return TokenKind.FractionUnit;
                case "deg": return TokenKind.DegreeUnit;
                case "rad": return TokenKind.RadianUnit;
                case "grad": return TokenKind.GradianUnit;
                case "turn": return TokenKind.TurnUnit;
                case "s": return TokenKind.SecondUnit;
                case "ms": return TokenKind.MillisecondUnit;
                case "hz": return TokenKind.HertzUnit;
                case "khz": return TokenKind.KilohertzUnit;
                case "dpi": return TokenKind.DotsPerInchUnit;
                case "dpcm": return TokenKind.DotsPerCentimeterUnit;
                case "dppx": return TokenKind.DotsPerPixelUnit;
                case "cap": return TokenKind.CapUnit;
                case "ic": return TokenKind.IcUnit;
                case "lh": return TokenKind.LhUnit;
                case "rlh": return TokenKind.RlhUnit;
                case "vi": return TokenKind.ViUnit;
                case "vb": return TokenKind.VbUnit;
                case "cqw": return TokenKind.ContainerQueryWidthUnit;
                case "cqh": return TokenKind.ContainerQueryHeightUnit;
                default: return TokenKind.Unknown;
            }
        }
    }
}
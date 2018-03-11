using System;
using System.Text;

namespace QMap
{
    public class Tokenizer
    {
        public enum TokenType
        {
            StartBlock,
            EndBlock,
            StartParen,
            EndParen,
            Value,
            EndOfStream
        }

        public class Token
        {
            public TokenType Type { get; private set; }
            public string Contents { get; private set; }
            public Token(TokenType type, string contents)
            {
                Type = type;
                Contents = contents;
            }

            // Pre-create some common tokens to reduce garbage
            public static Token StartBlock = new Token(TokenType.StartBlock, "{");
            public static Token EndBlock = new Token(TokenType.EndBlock, "}");
            public static Token StartParen = new Token(TokenType.StartParen, "(");
            public static Token EndParen = new Token(TokenType.EndParen, "}");
            public static Token EndOfStream = new Token(TokenType.EndOfStream, "}");

            public override string ToString()
            {
                return String.Format("{0}: {1}", Type, Contents);
            }
        }

        private string stringToTokenize = String.Empty;
        private int currentOffset = 0;

        public Tokenizer(string stringToTokenize)
        {
            this.stringToTokenize = stringToTokenize;
        }

        public Token GetNextToken()
        {
            while (currentOffset < stringToTokenize.Length)
            {
                // Move to the first non-whitespace character
                while (currentOffset < stringToTokenize.Length && Char.IsWhiteSpace(stringToTokenize[currentOffset]))
                    currentOffset++;

                if (currentOffset == stringToTokenize.Length) return Token.EndOfStream;

                // Grab the current character and immediately increment one to make other parts easier
                char c = stringToTokenize[currentOffset++];
                switch (c)
                {
                    case '{': return Token.StartBlock;
                    case '}': return Token.EndBlock;
                    case '(': return Token.StartParen;
                    case ')': return Token.EndParen;
                    case '/': // Assume start of comment
                        while (stringToTokenize[currentOffset] != '\r' && stringToTokenize[currentOffset] != '\n') currentOffset++;
                        break;
                    case '\"': // Quoted String as Value
                        StringBuilder sb = new StringBuilder();
                        while (stringToTokenize[currentOffset] != '\"') sb.Append(stringToTokenize[currentOffset++]);
                        currentOffset++; // Skip closing quote
                        return new Token(TokenType.Value, sb.ToString());
                    default: // Value
                        StringBuilder sb2 = new StringBuilder();
                        sb2.Append(c); // Make sure we add in the character we fetched first
                        while (!Char.IsWhiteSpace(stringToTokenize[currentOffset])) sb2.Append(stringToTokenize[currentOffset++]);
                        return new Token(TokenType.Value, sb2.ToString());
                }
            }
            return Token.EndOfStream;
        }

        public Token PeekNextToken()
        {
            int cachedOffset = currentOffset;
            var token = GetNextToken();
            currentOffset = cachedOffset;
            return token;
        }

        public string GetNextValue()
        {
            Token t = GetNextToken();
            if (t.Type != TokenType.Value)
            {
                throw new FormatException("Expected a value, got a " + t);
            }
            return t.Contents;
        }
    }
}

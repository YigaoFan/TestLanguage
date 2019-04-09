namespace ConfigureParser
{
    internal enum TokenType
    {
        Word,
        LeftBracket,
        RightBracket,
        LeftSquareBracket,
        RightSquareBracket,
    }

    internal class Token
    {
        public string Content { get; }
        public int Level { get; }
        public TokenType Type { get; }

        public Token(string content, int level, TokenType type)
        {
            Content = content;
            Level = level;
            Type = type;
        }
    }
}

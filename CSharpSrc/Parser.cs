using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigureParser
{
    public class Parser
    {
        private readonly Node headNode;
        private string name;

        private readonly string netCode;
        private List<Token> tokenList;

        public static Tree Parse(string code)
        {
            var p = new Parser(code);
            return new Tree(p.name, p.headNode);
        }

        private Parser(string code)
        {
            netCode = Parser.ReplaceLine(code);
            headNode = Tokenize().Check().DoParse(3); // 3 is the first element to parse
        }

        private Parser Tokenize()
        {
            tokenList = new List<Token>();
            var currentBuffer = new StringBuilder();
            var currentLevel = 0;

            foreach (var c in netCode)
            {
                switch (c)
                {
                    case '(':
                        ++currentLevel;
                        tokenList.Add(new Token("(", currentLevel, TokenType.LeftBracket));
                        break;
                    case ')':
                        if (currentBuffer.Length > 0) // (word)
                        {
                            tokenList.Add(new Token(currentBuffer.ToString(), currentLevel, TokenType.Word));
                            currentBuffer.Clear();
                        }
                        tokenList.Add(new Token(")", currentLevel, TokenType.RightBracket));
                        --currentLevel;
                        break;
                    case '[':
                        ++currentLevel;
                        tokenList.Add(new Token("[", currentLevel, TokenType.LeftSquareBracket));
                        break;
                    case ']':
                        tokenList.Add(new Token("]", currentLevel, TokenType.RightSquareBracket));
                        --currentLevel;
                        break;
                    case ' ': // TODO next time not use space to divide token
                        if (currentBuffer.Length > 0) // "  " two spaces
                        {
                            tokenList.Add(new Token(currentBuffer.ToString(), currentLevel, TokenType.Word));
                            currentBuffer.Clear();
                        }
                        break;
                    default:
                        if (!char.IsLetterOrDigit(c))
                        {
                            throw new ArgumentException("Code has illegal char: " + currentBuffer);
                        }
                        else
                        {
                            currentBuffer.Append(c);
                        }
                        break;
                }
                if (currentLevel < 0)
                {
                    throw new ArgumentException("Bracket is not pair: " + currentBuffer);
                }
            }

            if (currentLevel > 0)
            {
                throw new ArgumentException("Still have bracket is not paired");
            }
            return this;
        }

        private Parser Check()
        {
            // Process define
            if (tokenList[0].Content == "("
                && tokenList[1].Content.ToLower() == "define"
                && tokenList[2].Type == TokenType.Word)
            {
                name = tokenList[2].Content;
                if (tokenList[3].Type != TokenType.LeftBracket
                    && tokenList[3].Type != TokenType.LeftSquareBracket)
                {
                    throw new ArgumentException("After define name, it should be '(' or '[' there");
                }
            }
            // TODO check detail about pair is [], not like [)

            // check word place
            var lastType = tokenList[1].Type;
            var nextType = tokenList[3].Type;
            for (var i = 3; i < tokenList.Count - 1; ++i) // - 1 maybe occur some problem, some other grammar is not check, like () abc
            {
                var currentType = nextType;
                nextType = tokenList[i + 1].Type;
                if (currentType == TokenType.Word)
                {
                    if (lastType != TokenType.LeftBracket || nextType != TokenType.RightBracket)
                    {
                        throw new ArgumentException("The word can only be allowed between ( )");
                    }
                }
                lastType = currentType;
            }
            return this;
        }

        private Node DoParse(int i)
        {
            int pairedOne;
            switch (tokenList[i].Type)
            {
                case TokenType.LeftBracket:
                    pairedOne = FindPair(i, TokenType.RightBracket);
                    return ProcessBracket(i, pairedOne);
                case TokenType.LeftSquareBracket:
                    pairedOne = FindPair(i, TokenType.RightSquareBracket);
                    return ProcessSquareBracket(i, pairedOne);
                default:
                    // TODO should declare NAME in the document
                    throw new ArgumentException("Found the illegal element after the NAME");
            }
        }

        private Node ProcessBracket(int begin, int end)
        {
            switch (end - begin)
            {
                case 1: // () should in grammar check? TODO
                    throw new ArgumentException("The content between two paired bracket should not be empty");
                case 2: // (word)
                    return new Node(tokenList[begin + 1].Content);
                default: // like (()[])
                    Node last;
                    Node first = last = DoParse(begin + 1);
                    begin = FindPair(begin + 1);

                    while (begin < end - 1) // (()Z)
                    {
                        // there are some info lose between the two function
                        Node n = DoParse(begin + 1);
                        last.AddNode(n);
                        // update
                        last = n;
                        begin = FindPair(begin + 1); // actually begin point to the last pair's end
                    }
                    return first; // or should return last
            }
        }

        private Node ProcessSquareBracket(int begin, int end)
        {
            switch (end - begin)
            {
                case 1: // [] should in grammar check? TODO
                    throw new ArgumentException("The content between two paired bracket should not be empty");
                case 2: // [word]
                    return new Node(tokenList[begin + 1].Content);
                default: // like [()[]]
                    var n = new Node("");
                    while (begin < end - 1) // [()]
                    {
                        n.AddNode(DoParse(begin + 1));
                        begin = FindPair(begin + 1); // actually begin point to the last pair's end
                    }
                    return n;
            }
        }

        private int FindPair(int i, TokenType destType)
        {
            var srcLevel = tokenList[i].Level;
            for (; i < tokenList.Count; ++i)
            {
                if (tokenList[i].Type == destType && tokenList[i].Level == srcLevel)
                {
                    return i;
                }
            }
            throw new ArgumentException("Not found the paired one of " + tokenList[i].Content);
        }

        private int FindPair(int i)
        {
            switch (tokenList[i].Type)
            {
                case TokenType.LeftBracket:
                    return FindPair(i, TokenType.RightBracket);
                case TokenType.LeftSquareBracket:
                    return FindPair(i, TokenType.RightSquareBracket);
                default:
                    throw new ArgumentException("Not found the paired one of " + tokenList[i].Content);
            }
        }

        private static string ReplaceLine(string code)
        {
            return code
                .Replace("\n", " ")
                .Replace("\t", " ")
                .Replace("\r", " ");
        }
    }
}

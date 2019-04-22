using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigureParser
{
    public class Parser
    {
        private readonly Node _headNode;
        private string _name;

        private readonly string _netCode;
        private List<Token> _tokenList;
        private readonly List<Node> _nodes = new List<Node>(); // Convenient to equip operation, so just record the Not "" Node

        public static Tree Parse(string code)
        {
            var p = new Parser(code);
            return new Tree(p._name, p._headNode, p._nodes);
        }

        private Parser(string code)
        {
            _netCode = Parser.ReplaceLine(code);
            _headNode = Tokenize().Check().DoParseFrom(3, FindPair(3)).Head; // 3 is the first element to parse
        }

        private Parser Tokenize()
        {
            _tokenList = new List<Token>();
            var currentBuffer = new StringBuilder();
            var currentLevel = 0;

            foreach (var c in _netCode)
            {
                switch (c)
                {
                    case '(':
                        ++currentLevel;
                        _tokenList.Add(new Token("(", currentLevel, TokenType.LeftBracket));
                        break;
                    case ')':
                        if (currentBuffer.Length > 0) // (word)
                        {
                            _tokenList.Add(new Token(currentBuffer.ToString(), currentLevel, TokenType.Word));
                            currentBuffer.Clear();
                        }
                        _tokenList.Add(new Token(")", currentLevel, TokenType.RightBracket));
                        --currentLevel;
                        break;
                    case '[':
                        ++currentLevel;
                        _tokenList.Add(new Token("[", currentLevel, TokenType.LeftSquareBracket));
                        break;
                    case ']':
                        _tokenList.Add(new Token("]", currentLevel, TokenType.RightSquareBracket));
                        --currentLevel;
                        break;
                    case ' ': // TODO next time not use space to divide token
                        if (currentBuffer.Length > 0) // "  " two spaces
                        {
                            _tokenList.Add(new Token(currentBuffer.ToString(), currentLevel, TokenType.Word));
                            currentBuffer.Clear();
                        }
                        break;
                    default:
                        if (char.IsLetterOrDigit(c) || c == '.')
                        {
                            currentBuffer.Append(c);
                        }
                        else
                        {
                            throw new ArgumentException("Code has illegal char: " + currentBuffer);
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
            if (_tokenList[0].Content == "("
                && _tokenList[1].Content.ToLower() == "define"
                && _tokenList[2].Type == TokenType.Word)
            {
                _name = _tokenList[2].Content;
                if (_tokenList[3].Type != TokenType.LeftBracket
                    && _tokenList[3].Type != TokenType.LeftSquareBracket)
                {
                    throw new ArgumentException("After define name, it should be '(' or '[' there");
                }
            }
            // TODO check detail about pair is [], not like [)

            // check word place
            var lastType = _tokenList[1].Type;
            var nextType = _tokenList[3].Type;
            for (var i = 3; i < _tokenList.Count - 1; ++i) // - 1 maybe occur some problem, some other grammar is not check, like () abc
            {
                var currentType = nextType;
                nextType = _tokenList[i + 1].Type;
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

        /// <returns>The pair of head and tail of a linked list</returns>
        private HeadTail DoParseFrom(int start, int end)
        {
            switch (_tokenList[start].Type)
            {
                case TokenType.LeftBracket:
                    return ProcessBracket(start, end);
                case TokenType.LeftSquareBracket:
                    return ProcessSquareBracket(start, end);
                default:
                    // TODO should declare NAME in the document
                    throw new ArgumentException("Illegal character was found after the NAME");
            }
        }

        /// <returns>The pair of head and tail of a linked list</returns>
        private HeadTail ProcessBracket(int begin, int end)
        {
            switch (end - begin)
            {
                case 1: // () should in grammar check? TODO
                    throw new ArgumentException("The content between two paired bracket should not be empty");
                case 2: // (word)
                    return new HeadTail(MakeNewNode(_tokenList[begin + 1].Content));
                default: // like (()[])
                    var e = FindPair(begin + 1);
                    var first = DoParseFrom(begin + 1, e);
                    var tail = first.Tail;
                    begin = e;

                    while (begin < end - 1)
                    {
                        begin = begin + 1;
                        e = FindPair(begin);
                        var current = DoParseFrom(begin, e);

                        tail.AddChild(current.Head);
                        tail = current.Tail;
                        // update
                        begin = e;
                    }

                    return new HeadTail(first.Head, tail);
            }
        }

        /// <returns>The pair of head and tail of a linked list</returns>
        private HeadTail ProcessSquareBracket(int begin, int end)
        {
            switch (end - begin)
            {
                case 1: // [] should in grammar check? TODO
                    throw new ArgumentException("The content between two paired bracket should not be empty");
                case 2: // [word]
                    throw new ArgumentException("The content between two paired bracket should not be just one word directly");
                default: // like [()[]]
                    var head = MakeNewNode("");
                    var tail = MakeNewNode("");
                    
                    while (begin < end - 1) // [()]
                    {
                        begin = begin + 1;
                        var e = FindPair(begin);
                        var current = DoParseFrom(begin, e);
                        head.AddChild(current.Head);
                        current.Tail.AddChild(tail);

                        // update
                        begin = e;
                    }
                    return new HeadTail(head, tail);
            }
        }

        private int FindPair(int i, TokenType destType)
        {
            var srcLevel = _tokenList[i].Level;
            for (; i < _tokenList.Count; ++i)
            {
                if (_tokenList[i].Type == destType && _tokenList[i].Level == srcLevel)
                {
                    return i;
                }
            }
            throw new ArgumentException("Not found the paired one of " + _tokenList[i].Content);
        }

        private int FindPair(int i)
        {
            switch (_tokenList[i].Type)
            {
                case TokenType.LeftBracket:
                    return FindPair(i, TokenType.RightBracket);
                case TokenType.LeftSquareBracket:
                    return FindPair(i, TokenType.RightSquareBracket);
                default:
                    throw new ArgumentException("Not found the paired one of " + _tokenList[i].Content);
            }
        }

        private Node MakeNewNode(string content)
        {
            // use Linq to check if this content is existed in _node
            var n = new Node(content);
            if (content != "")
            {
                _nodes.Add(n);
            }
            return n;
        }

        private static string ReplaceLine(string code)
        {
            return code
                .Replace("\n", " ")
                .Replace("\t", " ")
                .Replace("\r", " ");
        }

        /// <summary>
        /// This class is to help code to be readable
        /// </summary>
        private class HeadTail
        {
            public Node Head { get; }
            public Node Tail { get; }

            public HeadTail(Node head, Node tail)
            {
                Head = head;
                Tail = tail;
            }

            public HeadTail(Node onlyOne)
            {
                Head = Tail = onlyOne;
            }
        }

    }
}

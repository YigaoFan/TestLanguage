using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigureParser
{
    public class Parser
    {
        private readonly Node _headNode;
        private string _name;

        private readonly string _netCode;
        private List<Token> _tokenList;
        private readonly List<Node> _nodes = new List<Node>();
        private Stack<Node> _nodeStack = new Stack<Node>();

        public static Tree Parse(string code)
        {
            var p = new Parser(code);
            return new Tree(p._name, p._headNode, p._nodes);
        }

        private Parser(string code)
        {
            _netCode = Parser.ReplaceLine(code);
            _headNode = Tokenize().Check().DoParseFrom(3); // 3 is the first element to parse
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

        private Node DoParseFrom(int i)
        {
            int pairedOne;
            switch (_tokenList[i].Type)
            {
                case TokenType.LeftBracket:
                    pairedOne = FindPair(i, TokenType.RightBracket);
                    return ProcessBracket(i, pairedOne);
                case TokenType.LeftSquareBracket:
                    pairedOne = FindPair(i, TokenType.RightSquareBracket);
                    return ProcessSquareBracket(i, pairedOne);
                default:
                    // TODO should declare NAME in the document
                    throw new ArgumentException("Illegal character was found after the NAME");
            }
        }

        private Node ProcessBracket(int begin, int end)
        {
            Node n;
            switch (end - begin)
            {
                case 1: // () should in grammar check? TODO
                    throw new ArgumentException("The content between two paired bracket should not be empty");
                case 2: // (word)
                    n = MakeNewNode(_tokenList[begin + 1].Content);
                    _nodes.Add(n);
                    return n;
                default: // like (()[])
                    var first = DoParseFrom(begin + 1);
                    begin = FindPair(begin + 1);

                    while (begin < end - 1) // (()Z)
                    {
                        // there are some info lose between the two function
                        n = DoParseFrom(begin + 1);

                        while (_nodeStack.Count != 0) // TODO should be moved to a method?
                        {
                            _nodeStack.Pop().AddChild(n); // error TODO should before DoParseFrom? it will affect?
                        }
                        //last.AddChild(n);
                        // update
                        //last = n;
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
                    return MakeNewNode(_tokenList[begin + 1].Content); // TODO should error?
                default: // like [()[]]
                    //var n = MakeNewNode("");
                    var lastNodes = new List<Node>();
                    while (_nodeStack.Count != 0)
                    {
                        lastNodes.Add(_nodeStack.Pop());
                    }
                    var lastNode = _nodeStack.Pop();
                    while (begin < end - 1) // [()]
                    {
                        var n = DoParseFrom(begin + 1);

                        foreach (var last in lastNodes)
                        {
                            last.AddChild(n); // TODO maybe occur some problem
                        }

                        begin = FindPair(begin + 1); // actually begin point to the last pair's end
                    }
                    return lastNode;
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
            var n = new Node(content);
            _nodes.Add(n);
            _nodeStack.Push(n);
            return n;
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

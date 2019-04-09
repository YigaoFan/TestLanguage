﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConfigureParser
{
    /// <summary>
    /// Node is in Tree, include a Content string and a callable delegate object.
    /// Can Enumerable.
    /// </summary>
    public class Node : IEnumerable
    {
        public delegate void TestOperation();
        public string Content { get; } // Content could be ""

        public TestOperation TestStep { get; set; } = () => { return; };

        public int ChildCount => _childNodes.Count; // Just read?

        private readonly List<Node> _childNodes;

        /// <param name="content">Content can not be null, it can be "" and "methodName"</param>
        public Node(string content)
        {
            Content = content;
            _childNodes = new List<Node>();
        }

        public Node AddChild(Node node)
        {
            _childNodes.Add(node);
            return this;
        }

        public Node this[int i] => _childNodes[i];

        public IEnumerator GetEnumerator()
        {
            return _childNodes.GetEnumerator();
        }
    }
}

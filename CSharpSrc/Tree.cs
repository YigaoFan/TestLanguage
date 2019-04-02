using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigureParser
{
    public class Tree
    {
        public string Name { get; }
        public Node HeadNode { get; }

        public Tree(string name, Node headNode)
        {
            Name = name;
            HeadNode = headNode;
        }
    }
}

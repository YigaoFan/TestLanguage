using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConfigureParser
{
    public class Tree
    {
        public string Name { get; }
        public Node HeadNode { get; }
        private Type _type;

        public Tree(string name, Node headNode)
        {
            Name = name;
            HeadNode = headNode;
            _type = Type.GetType(Name);
        }

        public Tree EquipOperations(Assembly asm = null) // reserved for future using
        {
            if (asm != null)
            {
                // use asm to re-assign the type variable
            }
            else
            {
                EquipRecursively(HeadNode);
            }

            return this;

        }

        public List<Node> searchRoute(string destStep)
        {
            var route = new List<Node>();
            // TODO
            return route;
        }

        private void EquipRecursively(Node node)
        {
            try
            {
                node.TestStep = () => _type.GetMethod(node.Content).Invoke(null, null);
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Set up the TestStep of node failed, please check the related thing");
                throw;
            }

            if (node.ChildCount != 0)
            {
                foreach (Node child in HeadNode)
                {
                    EquipRecursively(child);
                }
            }
        }
    }
}

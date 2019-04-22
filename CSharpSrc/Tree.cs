using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConfigureParser
{
    public class Tree
    {
        public string Name { get; }
        public Node HeadNode { get; }
        private readonly Type _type; // maybe use will change this _type
        private readonly List<Node> _allNodeWithRealOperation;

        public Tree(string name, Node headNode, List<Node> allNodeWithRealOperation)
        {
            // TODO default assembly scope is the first part of name
            if ((_type = Type.GetType(name + ", " + name.Split('.')[0])) == null)
            {
                throw new ArgumentException("Can't find the type: " + name);
            }

            Name = name;
            HeadNode = headNode;
            _allNodeWithRealOperation = allNodeWithRealOperation;
        }

        public Tree EquipOperations(Assembly asm = null) // reserved for future using
        {
            if (asm != null)
            {
                // use asm to re-assign the type variable
                throw new NotImplementedException();
            }
            else
            {
                foreach (var node in _allNodeWithRealOperation)
                {
                    try
                    {
                        var method = _type.GetMethod(node.Content);
                        if (method == null)
                        {
                            throw new ArgumentException("Can't find the method: " + node.Content);
                        }

                        // TODO now is static method
                        node.TestStep = () => method.Invoke(null, null);

                    }
                    catch (NullReferenceException e)
                    {
                        Console.WriteLine(e);
                        Console.WriteLine("Set up the TestStep of node failed, please check the related thing");
                        throw;
                    }
                }
                //EquipRecursively(HeadNode);
            }
            return this;
        }

        /// <returns>When no this step in the tree, return null</returns>
        public List<Node> SearchRoute(string destStep, List<int> choiceArray)
        {
            foreach (var n in _allNodeWithRealOperation)
            {
                if (n.Content == destStep)
                {
                    return ConstructRoute(destStep, choiceArray);
                }
            }

            throw new Exception("Some wrong, please check the destination step");
        }

        private List<Node> ConstructRoute(string destStep, IReadOnlyList<int> choiceArray)
        {
            var level = 0; // use to declare which level is the choice make
            var route = new List<Node>();

            var currentNode = HeadNode;
            Add(ref route, currentNode);

            if (currentNode.Content == destStep)
            {
                return route;
            }

            while (currentNode.ChildCount != 0)
            {
                var branchNum = 0;

                if (currentNode.ChildCount != 1)
                {
                    if (choiceArray == null)
                    {
                        throw new ArgumentException("Please provide choice array");
                    }


#if true
                    branchNum = choiceArray[level];
                    ++level;
                    goto Add;
#else
                    for (var i = 0; i < currentNode.ChildCount; ++i)
                    {
                        var child = currentNode[i];

                        if (child.Content == choiceArray[level])
                        {
                            branchNum = i;
                            ++level;
                            goto Add;
                        }
                    }
#endif
                throw new ArgumentException("Wrong choice array, please re-check it");
                }

                Add:
                currentNode = currentNode[branchNum];
                Add(ref route, currentNode);

                // check destination arrived
                if (currentNode.Content == destStep)
                {
                    break;
                }
            }

            return route;
        }

        private static void Add(ref List<Node> route, Node newNode)
        {
            if (newNode.Useful)
            {
                route.Add(newNode);
            }
        }
    }
}

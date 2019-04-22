using System;
using System.Collections.Generic;
using System.IO;

namespace ConfigureParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string code = null;
            try
            {
                using (var sr = new StreamReader("D:/Configure.scm"))
                {
                    code = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:" + e.Message);
            }

            if (code != null)
            {
                //var t = Parser.Parse(code);
                //var currentNode = t.HeadNode;
                //Trace.Assert(t.Name == "Word");
                //Trace.Assert(currentNode.Content == "Hello");
                //currentNode = currentNode[0];
                //Trace.Assert(currentNode.Content == "World");
                //currentNode = currentNode[0];
                //Trace.Assert(currentNode.Content == "");
                //Trace.Assert(currentNode[0].Content == "AndYou");
                //Trace.Assert(currentNode[1].Content == "AndMe");
                //Trace.Assert(currentNode.ChildCount == 2);
                //Debug.Assert();
                var t = new OperationTree(code);
                t.OperateTo("ViewAccount", new List<int> { 0 });
                Console.Read();
            }
        }
    }
}

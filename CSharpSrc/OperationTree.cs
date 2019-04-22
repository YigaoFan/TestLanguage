using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConfigureParser
{
    public class OperationTree
    {
        private readonly Tree _operationTree;

        /// <param name="configureCode"></param>
        /// <param name="asm">Specify a specific assembly file to load operation method. Default value is null</param>
        public OperationTree(string configureCode, Assembly asm = null)
        {
            if (asm != null)
            {
                // TODO asm mechanism still has some error, ie. asm is not equal to Class name
                throw new NotImplementedException("The specific asm feature will be completed in the future");
            }
            _operationTree = Parser.Parse(configureCode).EquipOperations(asm);
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destStep">Destination operation</param>
        /// <param name="choiceArray">This array is to store the name of branch structure's choice, default value is null </param>
        /// <returns>The name of terminated step, if same to destStep, means successful</returns>
        public void OperateTo(string destStep, List<int> choiceArray = null) // int type is temp
        {
            var route = _operationTree.SearchRoute(destStep, choiceArray);
            foreach (var step in route)
            {
                try
                {
                    step.TestStep();
                }
                catch (Exception e)
                {
                    Log.WriteLine("Have problem in performing " + step.Content + ":", e);
                    throw;
                }
            }
        }
    }
}

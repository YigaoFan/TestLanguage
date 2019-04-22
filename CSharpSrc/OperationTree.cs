using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FacilityLibrary;
using FacilityLibrary.Util;

namespace ConfigureParser
{
    public class OperationTree
    {
        private readonly Tree _operationTree;
        private readonly Type _type;
        // TODO should save a type info to create a instance of it

        /// <param name="configureCode"></param>
        /// <param name="asm">Specify a specific assembly file to load operation method. Default value is null</param>
        public OperationTree(string configureCode, Assembly asm = null)
        {
            if (asm != null)
            {
                // TODO asm mechanism still has some error, ie. asm is not equal to Class name
                throw new NotImplementedException("The specific asm feature will be completed in the future");
            }
            _operationTree = Parser.Parse(configureCode).EquipOperations(out _type, asm);
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destStep">Destination operation</param>
        /// <param name="choiceArray">This array is to store the name of branch structure's choice, default value is null </param>
        /// <returns>The name of terminated step, if same to destStep, means successful</returns>
        public ContinuedAction OperateTo(string destStep, List<int> choiceArray = null) // int type is temp
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

            var heritageActions = from p in _type.GetProperties() where p.Name == "HeritageActions" select p;
            return new ContinuedAction(heritageActions.First().GetValue(null, null) as List<Action>);
        }
    }
}

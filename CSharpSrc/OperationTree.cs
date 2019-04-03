using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigureParser
{
    class OperationTree
    {
        private Tree _operationTree;

        public OperationTree(string configureCode)
        {
            _operationTree = Parser.Parse(configureCode).EquipOperations();
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destStep"></param>
        /// <returns>The name of terminated step, if same to destStep, means successful</returns>
        public string OperateTo(string destStep) // para type is not sure TODO
        {
            var route = _operationTree.searchRoute(destStep);
            if (route != null && route.Count != 0)
            {
                foreach (var step in _operationTree.searchRoute(destStep))
                {
                    step.TestStep(); // TODO　Check if have some problem
                }
            }
            else
            {
                throw new Exception("Some wrong, please check");
            }


            return destStep;
        }
    }
}

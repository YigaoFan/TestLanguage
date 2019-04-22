using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigureParser
{
    public class ContinuedAction
    {
        private readonly List<Action> _actions;

        public ContinuedAction(List<Action> actions)
        {
            this._actions = actions;
        }

        ~ContinuedAction()
        {
            _actions.ForEach(a => a());
        }
    }
}

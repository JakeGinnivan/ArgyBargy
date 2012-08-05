using System;
using System.Windows.Input;

namespace ArgyBargy
{
    public class NegativeDetailsAction : DetailsAction
    {
        public NegativeDetailsAction(string text, string tooltip, Action commandAction, string automationId = null, bool isDefault = false)
            : base(text, tooltip, commandAction, automationId, isDefault)
        {
        }

        public NegativeDetailsAction(string text, string tooltip, Action commandAction, Func<bool> canExecuteCommandAction, string automationId = null, bool isDefault = false)
            : base(text, tooltip, commandAction, canExecuteCommandAction, automationId, isDefault)
        {
        }

        public NegativeDetailsAction(string text, string tooltip, ICommand command, string automationId = null, bool isDefault = false)
            : base(text, tooltip, command, automationId, isDefault)
        {
        }
    }
}
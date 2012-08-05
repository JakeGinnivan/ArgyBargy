using System;
using System.Windows.Input;

namespace ArgyBargy
{
    public class DetailsAction : DetailsActionBase
    {
        public DetailsAction(string text, string tooltip, Action commandAction, string automationId = null, bool isDefault = false)
            : base(text, tooltip, new DelegateCommand<object>(o => { if (commandAction != null) commandAction(); }), automationId, isDefault)
        {
        }

        public DetailsAction(string text, string tooltip, Action commandAction, Func<bool> canExecuteCommandAction, string automationId = null, bool isDefault = false)
            : base(text, tooltip, new DelegateCommand<object>(
                                      o => { if (commandAction != null) commandAction(); },
                                      o => canExecuteCommandAction == null || canExecuteCommandAction()), automationId, isDefault)
        {
        }

        public DetailsAction(string text, string tooltip, ICommand command, string automationId = null, bool isDefault = false)
            : base(text, tooltip, command, automationId, isDefault)
        {
        }
    }
}
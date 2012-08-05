using System;

namespace ArgyBargy
{
    public class CancelDetailsAction : NegativeDetailsAction
    {
        public CancelDetailsAction(string tooltip = "Cancels the current action", Action commandAction = null, string automationId = "Cancel", bool isDefault = false)
            : base("Cancel", tooltip, commandAction, null, automationId, isDefault)
        {
        }
    }
}
using System;

namespace ArgyBargy
{
    public class ActionsDialogueResult<T> : DialogueResult<T>
    {
        public string ButtonAutomationId { get; set; }

        public ActionsDialogueResult(string buttonAutomationId, T result) :
            base(result)
        {
            ButtonAutomationId = buttonAutomationId;
        }

        public ActionsDialogueResult(string buttonAutomationId, Exception ex) :
            base(ex)
        {
            ButtonAutomationId = buttonAutomationId;
        }

        private ActionsDialogueResult()
            : base(null)
        {
            ButtonAutomationId = "Cancel";
            Cancelled = true;
        }

        public new static ActionsDialogueResult<T> CancelledResult()
        {
            return new ActionsDialogueResult<T>();
        }
    }
}
namespace ArgyBargy
{
    public class ActionsDialogueResult : DialogueResult
    {
        public string ButtonAutomationId { get; set; }

        public ActionsDialogueResult(string buttonAutomationId)
        {
            ButtonAutomationId = buttonAutomationId;
        }

        private ActionsDialogueResult()
        {
            ButtonAutomationId = "Cancel";
            Cancelled = true;
        }

        public new static ActionsDialogueResult CancelledResult()
        {
            return new ActionsDialogueResult();
        }
    }
}
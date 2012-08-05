namespace ArgyBargy
{
    public interface IDialogueView : IView
    {
        void DialogueDisplayed(IDialogueWindow window);
        void DialogueClosed();
    }
}
using System;

namespace ArgyBargy
{
    public interface IDialogueViewWithoutResult : IDialogueView
    {
        event EventHandler<DialogueResultEventArgs> Finished;
    }
}
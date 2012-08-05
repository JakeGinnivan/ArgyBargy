using System;

namespace ArgyBargy
{
    public interface IDialogueView<TResult> : IDialogueView
    {
        event EventHandler<DialogueResultEventArgs<TResult>> Finished;
    }
}
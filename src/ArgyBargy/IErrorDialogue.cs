using System;

namespace ArgyBargy
{
    public interface IErrorDialogue : IDialogueViewWithoutResult
    {
        void Initialise(string message, Exception error);
    }
}
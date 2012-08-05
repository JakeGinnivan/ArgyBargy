using System;

namespace ArgyBargy
{
    public class DialogueResultEventArgs : EventArgs
    {
        public DialogueResultEventArgs(bool cancelled)
        {
            Cancelled = cancelled;
        }

        public bool Cancelled { get; private set; }

        public static DialogueResultEventArgs EmptyResult = new DialogueResultEventArgs(false);
        public static DialogueResultEventArgs CancelledResult = new DialogueResultEventArgs(true);
    }
}
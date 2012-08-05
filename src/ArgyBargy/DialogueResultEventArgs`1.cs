using System;

namespace ArgyBargy
{
    public class DialogueResultEventArgs<T> : EventArgs
    {
        public DialogueResultEventArgs(Exception error)
        {
            Result = new DialogueResult<T>(error);
        }

        public DialogueResultEventArgs(T result)
        {
            Result = new DialogueResult<T>(result);
        }

        public static DialogueResultEventArgs<T> Cancelled()
        {
            return new DialogueResultEventArgs<T> { Result = DialogueResult<T>.CancelledResult() };
        }

        private DialogueResultEventArgs() { }

        public DialogueResult<T> Result { get; private set; }
    }
}
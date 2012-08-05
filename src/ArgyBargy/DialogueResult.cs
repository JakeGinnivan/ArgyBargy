using System;
using System.Threading.Tasks;

namespace ArgyBargy
{
    public class DialogueResult
    {
        public DialogueResult()
        {
            WasError = false;
            Cancelled = false;
        }

        public DialogueResult(Exception exception)
        {
            WasError = true;
            Cancelled = false;
            Error = exception;
        }

        public bool WasError { get; private set; }
        public Exception Error { get; private set; }
        public bool Cancelled { get; protected set; }

        public static DialogueResult CancelledResult()
        {
            return new DialogueResult { Cancelled = true };
        }

        public static DialogueResult FromTask(Task asyncOperation)
        {
            asyncOperation.Wait();

            if (asyncOperation.IsCanceled)
                return CancelledResult();

            return asyncOperation.IsFaulted
                       ? new DialogueResult(asyncOperation.Exception)
                       : new DialogueResult();
        }
    }
}
using System;
using System.Threading.Tasks;

namespace ArgyBargy
{
    public class DialogueResult<T>
    {
        private DialogueResult()
        {
        }

        public DialogueResult(T result)
        {
            WasError = false;
            Cancelled = false;
            Result = result;
        }

        public DialogueResult(Exception exception)
        {
            WasError = true;
            Cancelled = false;
            Error = exception;
            Result = default(T);
        }

        public bool WasError { get; private set; }
        public Exception Error { get; private set; }
        public bool Cancelled { get; protected set; }
        public T Result { get; private set; }

        public static DialogueResult<T> CancelledResult()
        {
            return new DialogueResult<T> { Cancelled = true };
        }

        public static DialogueResult<T> FromBackgroundResult(Task<T> asyncOperation)
        {
            asyncOperation.Wait();

            if (asyncOperation.IsCanceled)
                return CancelledResult();

            return asyncOperation.IsFaulted
                       ? new DialogueResult<T>(asyncOperation.Exception)
                       : new DialogueResult<T>(asyncOperation.Result);
        }
    }
}
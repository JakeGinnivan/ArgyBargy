using System;
using System.Diagnostics;

namespace ArgyBargy
{
    public class TraceLogAdapter : ILogAdapter
    {
        public void Error(Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }

        public void Info(string message)
        {
            Trace.TraceInformation(message);
        }
    }
}
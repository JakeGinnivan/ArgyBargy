using System;

namespace ArgyBargy
{
    public interface ILogAdapter
    {
        void Error(Exception exception);
        void Info(string message);
    }
}
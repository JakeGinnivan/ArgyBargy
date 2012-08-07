using System;

namespace ArgyBargy.TestHelpers
{
    public class AssertFailedException : Exception
    {
        public AssertFailedException(string message) : base(message)
        {
        }
    }
}
using System;

namespace SimpleTesting.TestAdapter
{
    public class AssertionFailedException : Exception
    {
        public AssertionFailedException(string message)
            : base(message)
        {
        }
    }
}
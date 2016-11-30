using System;
using System.Threading.Tasks;

namespace SimpleTesting.TestAdapter
{
    public static class Assertions
    {
        public static void Assert(bool condition, string errorMessage = "Assertion failed")
        {
            if (!condition)
                throw new AssertionFailedException(errorMessage);
        }

        public static void Assert(bool? condition, string errorMessage = "Assertion failed")
        {
            if (!condition.HasValue || !condition.Value)
                throw new AssertionFailedException(errorMessage);
        }

        public static T GetException<T>(Action condition, string errorMessage = "Assertion failed")
            where T : Exception
        {
            try
            {
                condition();
                throw new AssertionFailedException(errorMessage);
            }
            catch (T exception)
            {
                return exception;
            }

            throw new AssertionFailedException(errorMessage);
        }

        public static async Task<T> GetExceptionAsync<T>(Func<Task> condition, string errorMessage = "Assertion failed")
            where T : Exception
        {
            try
            {
                await condition();
                throw new AssertionFailedException(errorMessage);
            }
            catch (T exception)
            {
                return exception;
            }

            throw new AssertionFailedException(errorMessage);
        }
    }
}
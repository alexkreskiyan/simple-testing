using MsTestResult = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult;

namespace SimpleTesting.TestAdapter
{
    internal class TestResultConverter
    {
        private readonly TestConverter testConverter;

        public TestResultConverter(TestConverter testConverter)
        {
            this.testConverter = testConverter;
        }

        public MsTestResult Convert(string source, Test test, TestResult testResult)
            => new MsTestResult(this.testConverter.Convert(source, test))
            {
                Duration = testResult.ExecutionDuration,
                StartTime = testResult.ExecutionStart,
                EndTime = testResult.ExecutionEnd,
                ErrorMessage = testResult.Failure?.Message,
                ErrorStackTrace = testResult.Failure?.StackTrace,
                Outcome = testResult.Outcome
            };
    }
}
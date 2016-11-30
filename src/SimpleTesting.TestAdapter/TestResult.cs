using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;

namespace SimpleTesting.TestAdapter
{
    internal class TestResult
    {
        public TestOutcome Outcome { get; set; } = TestOutcome.None;

        public Exception Failure { get; set; }

        public DateTimeOffset ExecutionStart { get; set; }

        public DateTimeOffset ExecutionEnd { get; set; }

        public TimeSpan ExecutionDuration { get; set; }

        public IList<string> Output { get; } = new List<string>();
    }
}

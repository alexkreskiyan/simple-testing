using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;

namespace SimpleTesting.TestAdapter
{
    internal class TestConverter
    {
        public TestCase Convert(string source, Test test)
            => new TestCase(test.FullyQualifiedName, new Uri(Constants.ExecutorUri), source)
            {
                CodeFilePath = test.File,
                DisplayName = test.DisplayName,
                LineNumber = test.Line
            };
    }
}
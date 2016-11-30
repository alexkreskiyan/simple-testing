using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace SimpleTesting.TestAdapter
{
    [ExtensionUri(Constants.ExecutorUri)]
    public class AdapterTestExecutor : ITestExecutor
    {
        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            if (runContext.IsBeingDebugged)
                Debugger.Launch();

            this.RunTestsAsync(
                  sources,
                  frameworkHandle,
                  new SimpleTestDiscoverer(),
                  new SimpleTestExecutor(),
                  new TestResultConverter(new TestConverter())
              )
                  .Wait();
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            if (runContext.IsBeingDebugged)
                Debugger.Launch();

            foreach (var test in tests)
                frameworkHandle.SendMessage(TestMessageLevel.Error, test.FullyQualifiedName);

            this.RunTestsAsync(
                tests,
                frameworkHandle,
                new SimpleTestDiscoverer(),
                new SimpleTestExecutor(),
                new TestResultConverter(new TestConverter())
            )
                .Wait();
        }

        public void Cancel()
        {
        }

        private async Task RunTestsAsync(
            IEnumerable<string> sources,
            IFrameworkHandle frameworkHandle,
            SimpleTestDiscoverer testDiscoverer,
            SimpleTestExecutor testExecutor,
            TestResultConverter testResultConverter
        )
            => await Task.WhenAll(sources.Select(
                source => this.RunAssemblyTestsAsync(
                    source,
                    frameworkHandle,
                    testDiscoverer,
                    testExecutor,
                    testResultConverter
                )
            ));

        private async Task RunTestsAsync(
            IEnumerable<TestCase> testCases,
            IFrameworkHandle frameworkHandle,
            SimpleTestDiscoverer testDiscoverer,
            SimpleTestExecutor testExecutor,
            TestResultConverter testResultConverter
        )
            => await Task.WhenAll(
                testCases
                .Select(testCase => testCase.Source)
                .Select(
                    source => this.RunAssemblyTestsAsync(
                        source,
                        testCases.Where(testCase => testCase.Source == source),
                        frameworkHandle,
                        testDiscoverer,
                        testExecutor,
                        testResultConverter
                    )
                )
            );


        private async Task RunAssemblyTestsAsync(
            string assemblyPath,
            IFrameworkHandle frameworkHandle,
            SimpleTestDiscoverer testDiscoverer,
            SimpleTestExecutor testExecutor,
            TestResultConverter testResultConverter
        )
        {
            var assemblyName = AssemblyLoadContext.GetAssemblyName(assemblyPath);
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName);

            var tests = await testDiscoverer.GetTests(assembly);

            await this.RunTestsAsync(
                assemblyPath,
                tests,
                frameworkHandle,
                testExecutor,
                testResultConverter
            );
        }

        private async Task RunAssemblyTestsAsync(
            string assemblyPath,
            IEnumerable<TestCase> testCases,
            IFrameworkHandle frameworkHandle,
            SimpleTestDiscoverer testDiscoverer,
            SimpleTestExecutor testExecutor,
            TestResultConverter testResultConverter
        )
        {
            var assemblyName = AssemblyLoadContext.GetAssemblyName(assemblyPath);
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName);

            var tests = await testDiscoverer.GetTests(assembly);
            tests = tests.Where(test => testCases.Any(testCase => testCase.FullyQualifiedName == test.FullyQualifiedName));

            await this.RunTestsAsync(
                assemblyPath,
                tests,
                frameworkHandle,
                testExecutor,
                testResultConverter
            );
        }

        private async Task RunTestsAsync(
            string source,
            IEnumerable<Test> tests,
            IFrameworkHandle frameworkHandle,
            SimpleTestExecutor testExecutor,
            TestResultConverter testResultConverter
        )
        {
            await testExecutor.RunTestsAsync(
                tests,
                (test, result) => frameworkHandle.RecordResult(
                    testResultConverter.Convert(source, test, result)
                )
            );
        }
    }
}
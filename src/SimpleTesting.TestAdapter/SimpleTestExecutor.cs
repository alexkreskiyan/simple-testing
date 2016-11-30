using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SimpleTesting.TestAdapter
{
    internal class SimpleTestExecutor
    {
        public async Task RunTestsAsync(
            IEnumerable<Test> tests,
            Action<Test, TestResult> handleResult
        )
        {
            var testsDictionary = this.GetClassesDictionary(tests);
            await Task.WhenAll(
                testsDictionary.Select(
                    pair => this.RunTestClassTestsAsync(
                        pair.Key,
                        pair.Value,
                        handleResult
                    )
                )
            );
        }

        private async Task RunTestClassTestsAsync(
            object testClassInstance,
            IEnumerable<Test> tests,
            Action<Test, TestResult> handleResult
        )
        {
            await Task.WhenAll(
                tests.Select(test => test.IsSkipped
                    ? this.HandleSkipped(test, handleResult)
                    : this.RunTestAsync(
                        testClassInstance,
                        test,
                        handleResult
                    )
                )
            );
        }

        private async Task RunTestAsync(
            object testClassInstance,
            Test test,
            Action<Test, TestResult> handleResult
        )
        {
            var testResult = new TestResult();
            var stopwatch = new Stopwatch();
            try
            {
                testResult.ExecutionStart = DateTimeOffset.Now;
                stopwatch.Start();
                var result = test.Method.Invoke(testClassInstance, Array.Empty<object>());
                if (result is Task)
                    await (result as Task);
                testResult.Outcome = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestOutcome.Passed;
            }
            catch (TargetInvocationException exception)
            {
                this.HandleException(testResult, exception.InnerException);
            }
            catch (Exception exception)
            {
                this.HandleException(testResult, exception);
            }
            finally
            {
                stopwatch.Stop();
                testResult.ExecutionEnd = DateTimeOffset.Now;
                testResult.ExecutionDuration = stopwatch.Elapsed;

                handleResult(test, testResult);
            }
        }

        private Task HandleSkipped(Test test, Action<Test, TestResult> handleResult)
        {
            var testResult = new TestResult()
            {
                Outcome = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestOutcome.Skipped
            };

            handleResult(test, testResult);

            return Task.CompletedTask;
        }

        private void HandleException(TestResult testResult, Exception exception)
        {
            testResult.Outcome = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestOutcome.Failed;
            testResult.Failure = exception;
        }

        private IReadOnlyDictionary<object, IEnumerable<Test>> GetClassesDictionary(IEnumerable<Test> tests)
        {
            var typesDictionary = new Dictionary<Type, object>();
            var interimDictionary = new Dictionary<object, List<Test>>();
            foreach (var test in tests)
            {
                var testClass = test.Method.DeclaringType;
                if (typesDictionary.ContainsKey(testClass))
                    interimDictionary[typesDictionary[testClass]].Add(test);
                else
                {
                    var instance = typesDictionary[testClass] = Activator.CreateInstance(testClass);
                    interimDictionary[instance] = new List<Test>() { test };
                }
            }

            return interimDictionary
                .ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value as IEnumerable<Test>
                );
        }
    }
}
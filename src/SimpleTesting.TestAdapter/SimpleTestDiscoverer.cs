using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SimpleTesting.TestAdapter
{
    internal class SimpleTestDiscoverer
    {
        public Task<IEnumerable<Test>> GetTests(Assembly assembly)
            => Task.FromResult<IEnumerable<Test>>(
                assembly
                .GetTypes()
                .AsParallel()
                .WithDegreeOfParallelism(Environment.ProcessorCount)
                .SelectMany(this.GetTestClassTests)
                .ToArray()
            );

        private IEnumerable<Test> GetTestClassTests(Type testClass)
            => testClass.GetMethods()
                .Where(this.IsTest)
                .Select(method => new Test(method))
                .ToArray();

        private bool IsTest(MethodInfo candidate)
            => candidate.GetCustomAttribute<FactAttribute>() != null &&
                !candidate.IsGenericMethod &&
                candidate.GetParameters().Length == 0;

        private bool IsNotSkipped(MethodInfo candidate)
            => candidate.GetCustomAttribute<SkipAttribute>() == null;
    }
}
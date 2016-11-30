using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace SimpleTesting.TestAdapter
{
    [FileExtension(Constants.FileExtensionDll)]
    [FileExtension(Constants.FileExtensionExe)]
    [DefaultExecutorUri(Constants.ExecutorUri)]
    public class AdapterTestDiscoverer : ITestDiscoverer
    {
        public void DiscoverTests(
            IEnumerable<string> sources,
            IDiscoveryContext discoveryContext,
            IMessageLogger logger,
            ITestCaseDiscoverySink discoverySink
        )
            => this.DiscoverSourcesAsync(
                sources,
                logger,
                discoverySink,
                new SimpleTestDiscoverer(),
                new TestConverter()
            )
                .Wait();

        private async Task DiscoverSourcesAsync(
            IEnumerable<string> sources,
            IMessageLogger logger,
            ITestCaseDiscoverySink discoverySink,
            SimpleTestDiscoverer testDiscoverer,
            TestConverter testConverter
        )
            => await Task.WhenAll(sources.Select(
                source => this.DiscoverAssemblyTestsAsync(
                    source, logger, discoverySink, testDiscoverer, testConverter
                )
            ));

        private async Task DiscoverAssemblyTestsAsync(
            string assemblyPath,
            IMessageLogger logger,
            ITestCaseDiscoverySink discoverySink,
            SimpleTestDiscoverer testDiscoverer,
            TestConverter testConverter
        )
        {
            var assemblyName = AssemblyLoadContext.GetAssemblyName(assemblyPath);
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName);

            foreach (var test in await testDiscoverer.GetTests(assembly))
                discoverySink.SendTestCase(testConverter.Convert(assemblyPath, test));
        }
    }
}
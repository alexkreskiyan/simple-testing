using SimpleTesting.TestAdapter;
using System.Threading.Tasks;
using static SimpleTesting.TestAdapter.Assertions;

namespace SimpleTesting.TestAdapter.Tests
{
    public class SampleTestClass
    {
        [Fact]
        public void Test_Succeed()
        {
            Assert(true);
        }

        [Fact]
        public void Test_Failing()
        {
            Assert(false);
        }

        [Fact]
        [Skip]
        public void Test_Skipped()
        {

        }

        [Fact]
        public async Task TestAsync()
        {
            await Task.CompletedTask;
            Assert(true);
        }
    }
}
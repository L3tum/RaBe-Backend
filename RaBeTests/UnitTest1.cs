using NUnit.Framework;

namespace RaBeTests
{
    public class Tests : GherkinSpec.NUnit.GherkinSpecBase
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}
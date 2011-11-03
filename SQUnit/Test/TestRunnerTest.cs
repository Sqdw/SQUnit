using NUnit.Framework;
using System.Linq;

namespace SQUnit.Test
{
	[TestFixture]
	public class TestRunnerTest
	{
		TestRunner _runner;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			_runner = new TestRunner();
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			_runner.Dispose();
		}

		[Test]
		public void RunsPassingTest()
		{
			var results = _runner.RunTestsInFile("TestPages/OnePassingTest.html").ToArray();

			Assert.That(results, Has.Length.EqualTo(1));
			var result = results[0];
			Assert.That(result.Passed, Is.True, "Passed");
			Assert.That(result.FileName, Is.EqualTo("TestPages/OnePassingTest.html"));
			Assert.That(result.TestName, Is.EqualTo("a passing test"));
			Assert.That(result.Message, Is.EqualTo(string.Empty));
		}

		[Test]
		public void RunsFailingTest()
		{
			var results = _runner.RunTestsInFile("TestPages/OneFailingTest.html").ToArray();

			Assert.That(results, Has.Length.EqualTo(1));
			var result = results[0];
			Assert.That(result.Passed, Is.False, "Passed");
			Assert.That(result.FileName, Is.EqualTo("TestPages/OneFailingTest.html"));
			Assert.That(result.TestName, Is.EqualTo("a failing test"));
			Assert.That(result.Message, Is.EqualTo("failed"));
		}
	}
}
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace SQUnit.Test
{
	public class JasmineTest
	{
		const string RunnerHtmlFilePath = "Jasmine/runner.html";

		const string FailingTestFilePath = "Jasmine/OneFailingTest.js";
		const string EmptyTestFilePath = "EmptyTest.html";
		const string InfiniteTestFilePath = "InfiniteTest.html";

		TestRunner _runner;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			_runner = new TestRunner(JasmineTestSuite.FactoryDelegate);
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			_runner.Dispose();
		}

		[SetUp]
		public void SetUp()
		{
			_runner.MaxWaitInMs = 10000;
		}

		[Test]
		public void DetectsFailingTest()
		{
			var testFile = CreateTestFileFor(FailingTestFilePath);

			var results = _runner.RunTestsInFile(testFile).ToArray();

			Assert.That(results, Has.Length.EqualTo(1), "Number of test results");
			var result = results[0];
			Assert.That(result.Passed, Is.False, "Passed");
			Assert.That(result.FileName, Is.EqualTo(testFile));
			Assert.That(result.TestName, Is.EqualTo("one failing test suite fails once."));
			Assert.That(result.Message, Is.EqualTo("Expected true to be false."));
		}

		[Test]
		[Ignore]
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
		[Ignore]
		public void ThrowsExceptionAndCreatesSnapshotWhenTestListElementIsMissing()
		{
			Assert.Throws<InvalidTestFileException>(() => _runner.RunTestsInFile(EmptyTestFilePath).ToArray());
			var imageFile = Path.ChangeExtension(EmptyTestFilePath, "png");
			Assert.That(File.Exists(imageFile), "screenshot file exists");
		}

		[Test]
		[Ignore]
		public void ReturnsFailureWhenSlowTestDoesNotFinishWithinTimeLimit()
		{
			_runner.MaxWaitInMs = 100;
			var results = _runner.RunTestsInFile(InfiniteTestFilePath).ToArray();
			Assert.That(results, Has.Length.EqualTo(1));
			Assert.That(results[0].Passed, Is.False);
		}

		string CreateTestFileFor(string jsTestFile)
		{
			var testFilePath = Path.ChangeExtension(jsTestFile, "html");
			var template = File.ReadAllText(RunnerHtmlFilePath);
			var content = template.Replace("$$TESTFILE$$", Path.GetFileName(jsTestFile));
			File.WriteAllText(testFilePath, content);
			return testFilePath;
		}
	}
}
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace SQUnit.Test
{
	public class JasmineTest
	{
		const string RunnerHtmlFilePath = "Jasmine/runner.html";

		const string FailingTestFilePath = "Jasmine/OneFailingTest.js";
		const string PassingTestFilePath = "Jasmine/OnePassingTest.js";
		const string EmptyTestFilePath = "Jasmine/EmptyTest.js";
		const string SlowTestFilePath = "Jasmine/SlowTest.js";

		TestRunner _runner;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			_runner = new TestRunner(JasmineTestSuite.Factory);
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
		public void DetectsFailedTest()
		{
			var testFile = CreateTestFileFor(FailingTestFilePath);

			var results = _runner.RunTestsInFile(testFile).ToArray();

			Assert.That(results, Has.Length.EqualTo(1), "Number of test results");
			var result = results[0];
			Assert.That(result.Passed, Is.False, "Passed");
			Assert.That(result.FileName, Is.EqualTo(testFile));
			Assert.That(result.TestName, Is.EqualTo("One failing test suite fails once."));
			Assert.That(result.Message, Is.EqualTo("Expected true to be false."));
		}

		[Test]
		public void DetectsPassedTest()
		{
			var testFile = CreateTestFileFor(PassingTestFilePath);

			var results = _runner.RunTestsInFile(testFile).ToArray();

			Assert.That(results, Has.Length.EqualTo(1));
			var result = results[0];
			Assert.That(result.Passed, Is.True, "Passed");
			Assert.That(result.FileName, Is.EqualTo(testFile));
			Assert.That(result.TestName, Is.EqualTo("One passing test suite passes once."));
			Assert.That(result.Message, Is.EqualTo(string.Empty));
		}

		[Test]
		public void ThrowsExceptionAndCreatesSnapshotWhenTestListElementIsMissing()
		{
			var testFile = CreateTestFileFor(EmptyTestFilePath);

			TestDelegate action = () => _runner.RunTestsInFile(testFile).ToArray();

			Assert.Throws<InvalidTestFileException>(action);
			var imageFile = Path.ChangeExtension(testFile, "png");
			Assert.That(File.Exists(imageFile), "screenshot file exists");
		}

		[Test]
		public void WaitsForTestToComplete()
		{
			_runner.MaxWaitInMs = 3000;
			var testFile = CreateTestFileFor(SlowTestFilePath);
			var stopwatch = new Stopwatch();

			stopwatch.Start();
			var results = _runner.RunTestsInFile(testFile).ToArray();
			stopwatch.Stop();

			Assert.That(results, Has.Length.EqualTo(1));
			Assert.That(results[0].Passed, Is.True);
			Assert.That(stopwatch.ElapsedMilliseconds, Is.GreaterThan(800));
		}

		[Test]
		public void ReturnsFailureWhenSlowTestDoesNotFinishWithinTimeLimit()
		{
			_runner.MaxWaitInMs = 100;
			var testFile = CreateTestFileFor(SlowTestFilePath);

			var results = _runner.RunTestsInFile(testFile).ToArray();

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
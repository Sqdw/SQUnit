﻿using System.IO;
using NUnit.Framework;
using System.Linq;

namespace SQUnit.Test
{
	[TestFixture]
	public class TestRunnerTest
	{
		const string FailingTestFilePath = "TestPages/OneFailingTest.html";
		const string EmptyTestFilePath = "TestPages/EmptyTest.html";

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
			var results = _runner.RunTestsInFile(FailingTestFilePath).ToArray();

			Assert.That(results, Has.Length.EqualTo(1));
			var result = results[0];
			Assert.That(result.Passed, Is.False, "Passed");
			Assert.That(result.FileName, Is.EqualTo(FailingTestFilePath));
			Assert.That(result.TestName, Is.EqualTo("a failing test"));
			Assert.That(result.Message, Is.EqualTo("failed"));
		}

		[Test]
		public void CreatesScreenshopOfFailedTest()
		{
			_runner.RunTestsInFile(FailingTestFilePath).ToArray();

			var imageFile = Path.ChangeExtension(FailingTestFilePath, "png");
			Assert.That(File.Exists(imageFile), "screenshot file exists");
		}

		[Test]
		public void ThrowsExceptionAndCreatesSnapshotWhenTestListElementIsMissing()
		{
			Assert.Throws<InvalidTestFileException>(() => _runner.RunTestsInFile(EmptyTestFilePath).ToArray());
			var imageFile = Path.ChangeExtension(EmptyTestFilePath, "png");
			Assert.That(File.Exists(imageFile), "screenshot file exists");
		}
	}
}
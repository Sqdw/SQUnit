using System;
using System.Collections.Generic;
using System.Linq;

namespace SQUnit
{
	public static class TestResultExtensions
	{
		public static void AssertTestPassed(this TestResult testResult)
		{
			if (testResult.Passed) return;
			ThrowTestFailure(testResult.GetTestFailureDescription());
		}

		public static void AssertAllTestsPassed(this IEnumerable<TestResult> testResults)
		{
			var failedTests = testResults.Where(tc => !tc.Passed).ToArray();
			if (!failedTests.Any()) return;
			
			var errorMessages = failedTests
				.Select(GetTestFailureDescription)
				.Aggregate((first, second) => string.Format("{0}{1}{1}{2}", first, Environment.NewLine, second));
			
			ThrowTestFailure(errorMessages);
		}

		static string GetTestFailureDescription(this TestResult testResult)
		{
			return string.Format(
				"{0}{1}== Screenshot: {2} ==",
				testResult.GetDescription(),
				Environment.NewLine,
				testResult.ScreenshotPath);
		}

		static void ThrowTestFailure(string testFailureDescription)
		{
			var message = string.Format("QUnit test(s) failed:{0}{1}", Environment.NewLine, testFailureDescription);
			throw new ApplicationException(message);
		}
	}
}
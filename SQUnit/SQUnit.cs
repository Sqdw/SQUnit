using System.Collections.Generic;
using System.Linq;

namespace SQUnit
{
	/// <summary>
	/// Entry class for parsing and returning QUnit tests.
	/// </summary>
	public class SQUnit
	{
		/// <summary>
		/// Returns an array of QUnitTestResult objects that encapsulate the QUnit tests within the passed in files to test.
		/// Will wait for infinity for any asynchronous tests to run.
		/// </summary>
		/// <param name="filesToTest">A list of one or more files to run tests on relative to the root of the test project.</param>
		/// <returns>An array of QUnitTestResult objects encapsulating the QUnit tests in the given files</returns>
		public static IEnumerable<QUnitTestResult> GetTestResults(params string[] filesToTest)
		{
			return GetTestResults(-1, filesToTest);
		}

		/// <summary>
		/// Returns an array of QUnitTestResult objects that encapsulate the QUnit tests within the passed in files to test.
		/// </summary>
		/// <param name="maxWaitInMs">The maximum number of milliseconds before the tests should timeout after page load; -1 for infinity, 0 to not support asynchronous tests</param>
		/// <param name="filesToTest">A list of one or more files to run tests on relative to the root of the test project.</param>
		/// <returns>An array of QUnitTestResult objects encapsulating the QUnit tests in the given files</returns>
		public static IEnumerable<QUnitTestResult> GetTestResults(int maxWaitInMs, params string[] filesToTest)
		{
			using (var qUnitParser = new QUnitParser(maxWaitInMs))
			{
				return filesToTest.SelectMany(qUnitParser.GetQUnitTestResults).ToArray();
			}
		}
	}
}

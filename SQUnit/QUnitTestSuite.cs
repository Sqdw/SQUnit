using System.Linq;
using OpenQA.Selenium;

namespace SQUnit
{
	public class QUnitTestSuite : TestSuiteBase
	{
		IWebElement _qunitTestsElement;

		public static readonly TestSuiteFactoryDelegate FactoryDelegate = (driver, path) => new QUnitTestSuite(driver, path);

		public QUnitTestSuite(IWebDriver driver, string testFilePath) 
			: base(driver, testFilePath)
		{
		}

		public override void Update()
		{
			var elements = Driver.FindElements(By.Id("qunit-tests")).ToArray();
			if (elements.Length == 0)
			{
				SaveScreenShot();
				var msg = string.Format(
					"The test file is missing the list of qunit tests - element '#qunit-tests' was not found. See [{0}] for details.",
					ScreenshotPath);
				throw new InvalidTestFileException(msg);
			}
			_qunitTestsElement = elements[0];
		}

		public override bool IsRunning()
		{
			return _qunitTestsElement.FindElements(By.CssSelector("li.running")).Any();
		}

		public override TestResult[] GetTestResults()
		{
			return _qunitTestsElement
				.FindElements(By.CssSelector("li[id^='test-output']"))
				.Select(ParseTestResult)
				.ToArray();
		}


		TestResult ParseTestResult(IWebElement testOutput)
		{
			var testName = testOutput.FindElement(By.ClassName("test-name")).Text;
			var resultClass = testOutput.GetAttribute("class");

			if (resultClass == "pass")
				return CreateTestResult(testName, true, string.Empty);

			if (resultClass == "fail")
				return CreateTestResult(testName, false, testOutput.FindElement(By.ClassName("fail")).Text);

			if (resultClass == "running")
				return CreateTestResult(testName, false, "The test did not finish within time limit.");

			return CreateTestResult(testName, false, "Unknown test class: '" + resultClass + "'");
		}
	}
}
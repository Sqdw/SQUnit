using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace SQUnit
{
	public class JasmineTestSuite : TestSuiteBase
	{
		IWebElement _htmlReporterResults;

		public static readonly TestSuiteFactoryDelegate FactoryDelegate = (driver, path) => new JasmineTestSuite(driver, path);

		public JasmineTestSuite(IWebDriver driver, string testFilePath) 
			: base(driver, testFilePath)
		{
		}

		public override void Update()
		{
			var elements = Driver.FindElements(By.CssSelector(".jasmine_reporter .results")).ToArray();
			if (elements.Length == 0)
			{
				SaveScreenShot();
				var msg = string.Format(
					"The test file is missing output of Jasmine HTML reporter - element '.jasmine_reporter .results' was not found. See [{0}] for details.",
					ScreenshotPath);
				throw new InvalidTestFileException(msg);
			}
			_htmlReporterResults = elements[0];
		}

		public override bool IsRunning()
		{
			return false;
			// TODO
			// return _qunitTestsElement.FindElements(By.CssSelector("li.running")).Any();
		}

		public override TestResult[] GetTestResults()
		{
			return _htmlReporterResults
				.FindElements(By.CssSelector(".specDetail"))
				.Select(ParseSpecDetail)
				.ToArray();
		}


		TestResult ParseSpecDetail(IWebElement specDetail)
		{
			var testName = specDetail.FindElement(By.CssSelector("a.description")).GetAttribute("title");
			
			var resultMessage = specDetail.FindElement(By.CssSelector(".resultMessage"));
			var resultClasses = ParseResultMessageClasses(resultMessage).ToArray();

			/*
			if (resultClass == "pass")
				return CreateTestResult(testName, true, string.Empty);
			*/

			if (resultClasses.Contains("fail"))
				return CreateTestResult(testName, false, resultMessage.Text);

			/*
			if (resultClass == "running")
				return CreateTestResult(testName, false, "The test did not finish within time limit.");
			*/

			return CreateTestResult(testName, false, "Unknown test class: '" + string.Join(" ", resultClasses) + "'");
		}

		static IEnumerable<string> ParseResultMessageClasses(IWebElement resultMessage)
		{
			return resultMessage
				.GetAttribute("class")
				.Split(' ')
				.Where(c => c != "resultMessage");
		}
	}
}
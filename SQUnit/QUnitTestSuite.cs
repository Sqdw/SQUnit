using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenQA.Selenium;

namespace SQUnit
{
	public class QUnitTestSuite
	{
		readonly IWebDriver _driver;
		readonly string _testFilePath;
		IWebElement _qunitTestsElement;

		public QUnitTestSuite(IWebDriver driver, string testFilePath)
		{
			_driver = driver;
			_testFilePath = testFilePath;
			_driver.Navigate().GoToUrl(Path.GetFullPath(_testFilePath));
		}

		public void Update()
		{
			var elements = _driver.FindElements(By.Id("qunit-tests")).ToArray();
			if (elements.Length == 0)
				throw new InvalidOperationException("No QUnit tests were found in the test file.");
			_qunitTestsElement = elements[0];
		}

		public bool IsRunning()
		{
			return _qunitTestsElement.FindElements(By.CssSelector("li.running")).Any();
		}

		public IEnumerable<TestResult> GetTestResults()
		{
			return _qunitTestsElement
				.FindElements(By.CssSelector("li[id^='test-output']"))
				.Select(ParseTestResult);
		}


		TestResult ParseTestResult(IWebElement testOutput)
		{
			var testName = testOutput.FindElement(By.ClassName("test-name")).Text;
			var resultClass = testOutput.GetAttribute("class");
			
			var passed = resultClass == "pass";
			var message = passed
			              	? string.Empty
			              	: testOutput.FindElement(By.ClassName("fail")).Text;

			return new TestResult
			{
				FileName = _testFilePath,
				TestName = testName,
				Passed = passed,
				Message = message
			};
		}
	}
}
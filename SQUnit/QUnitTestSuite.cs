using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
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

		static readonly ImageFormat ScreenshotFormat = ImageFormat.Png;
		string ScreenshotPath { get { return Path.ChangeExtension(_testFilePath, "png"); } }

		public QUnitTestSuite(IWebDriver driver, string testFilePath)
		{
			if (!File.Exists(testFilePath))
				throw new FileNotFoundException("The test file '" + testFilePath + "'was not found.", testFilePath);

			_driver = driver;
			_testFilePath = testFilePath;
			_driver.Navigate().GoToUrl(Path.GetFullPath(_testFilePath));
		}

		public void Update()
		{
			var elements = _driver.FindElements(By.Id("qunit-tests")).ToArray();
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
			
			if (resultClass == "pass")
				return CreateTestResult(testName, true, string.Empty);

			if (resultClass == "fail")
				return CreateTestResult(testName, false, testOutput.FindElement(By.ClassName("fail")).Text);

			if (resultClass == "running")
				return CreateTestResult(testName, false, "The test did not finish within time limit.");

			return CreateTestResult(testName, false, "Unknown test class: '" + resultClass + "'");
		}

		TestResult CreateTestResult(string testName, bool passed, string message)
		{
			return new TestResult
				{
					FileName = _testFilePath,
					TestName = testName,
					Passed = passed,
					Message = message,
					ScreenshotPath = ScreenshotPath
				};
		}

		public void SaveScreenShot()
		{
			((ITakesScreenshot) _driver).GetScreenshot().SaveAsFile(ScreenshotPath, ScreenshotFormat);
		}
	}
}
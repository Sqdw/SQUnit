﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;

namespace SQUnit
{
	public class TestRunner : IDisposable
	{
		IWebDriver _driver;
		int _maxWaitInMs = 10000;

		public TestRunner()
		{
			_driver = CreateFirefoxDriver();
		}

		static IWebDriver CreateInternetExplorerDriver()
		{
			// Note: InternetExplorer driver throws exceptions and doesn't close Browser window
			// It is not recommended to use it.
			var capabilities = DesiredCapabilities.InternetExplorer();
			capabilities.SetCapability("ignoreProtectedModeSettings", true);
			return new InternetExplorerDriver(capabilities);
		}

		static IWebDriver CreateFirefoxDriver()
		{
			return new FirefoxDriver();
		}

		public IEnumerable<TestResult> RunTestsInFile(string filePath)
		{
			_driver.Navigate().GoToUrl(Path.GetFullPath(filePath));
			return GrabTestResultsFromWebPage(filePath);
		}

		IEnumerable<TestResult> GrabTestResultsFromWebPage(string testPage)
		{
			var stillRunning = true;
			IWebElement testList = null;
			var wait = 0;

			// BEWARE: This logic is tightly coupled to the structure of the HTML generated by the QUnit test runner

			while (stillRunning && wait <= _maxWaitInMs)
			{
				var webElements = _driver.FindElements(By.Id("qunit-tests"));
				var elements = webElements.ToArray();
				if (elements.Length == 0)
					throw new InvalidOperationException("No QUnit tests were found in the test file.");
				testList = elements[0];

				stillRunning = testList.FindElements(By.TagName("li")).Any(e => e.GetAttribute("class") == "running");

				if (stillRunning && wait < _maxWaitInMs) Thread.Sleep(100);
				wait += 100;
			}

			foreach (var testOutput in testList.FindElements(By.CssSelector("li[id^='test-output']")))
			{
				var testName = testOutput.FindElement(By.ClassName("test-name")).Text;
				var resultClass = testOutput.GetAttribute("class");
				var failedAssert = string.Empty;
				if (resultClass == "fail")
				{
					failedAssert = testOutput.FindElement(By.ClassName("fail")).Text;
				}

				yield return new TestResult
				{
					FileName = testPage,
					TestName = testName,
					Passed = resultClass == "pass",
					Message = failedAssert
				};
			}

		}

		#region IDisposable

		public virtual void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_driver != null)
			{
				_driver.Close();
				_driver.Dispose();
				_driver = null;
			}
		}

		~TestRunner()
		{
			Dispose(false);
		}

		#endregion
	}
}
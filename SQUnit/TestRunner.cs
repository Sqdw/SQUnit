using System;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;

namespace SQUnit
{
	public class TestRunner : IDisposable
	{
		const int PollingIntervalInMs = 100;

		IWebDriver _driver;
		QUnitTestSuite _testSuite;

		public TestRunner() : this(CreateFirefoxDriver())
		{
		}

		public TestRunner(IWebDriver driver)
		{
			MaxWaitInMs = 10000;
			_driver = driver;
		}

		public int MaxWaitInMs { get; set; }

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

		public TestResult[] RunTestsInFile(string filePath)
		{
			_testSuite = new QUnitTestSuite(_driver, filePath);
			WaitForTestsToFinish();
			_testSuite.SaveScreenShot();
			return _testSuite.GetTestResults();
		}

		void WaitForTestsToFinish()
		{
			var remainingTimeInMs = MaxWaitInMs;

			while (remainingTimeInMs > 0)
			{
				_testSuite.Update();

				if (!_testSuite.IsRunning())
					break;

				Thread.Sleep(PollingIntervalInMs);
				remainingTimeInMs -= PollingIntervalInMs;
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
			if (_driver == null) return;
			_driver.Close();
			_driver.Dispose();
			_driver = null;
		}

		~TestRunner()
		{
			Dispose(false);
		}

		#endregion
	}
}
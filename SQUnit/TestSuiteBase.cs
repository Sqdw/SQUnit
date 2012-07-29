using System.Drawing.Imaging;
using System.IO;
using OpenQA.Selenium;

namespace SQUnit
{
	public abstract class TestSuiteBase : ITestSuite
	{
		static readonly ImageFormat ScreenshotFormat = ImageFormat.Png;

		protected IWebDriver Driver { get; private set; }
		protected string TestFilePath { get; private set; }
		protected string ScreenshotPath { get { return Path.ChangeExtension(TestFilePath, "png"); } }

		protected TestSuiteBase(IWebDriver driver, string testFilePath)
		{
			if (!File.Exists(testFilePath))
				throw new FileNotFoundException("The test file '" + testFilePath + "'was not found.", testFilePath);

			Driver = driver;
			TestFilePath = testFilePath;

			Driver.Navigate().GoToUrl(Path.GetFullPath(TestFilePath));
		}

		public abstract void Update();
		public abstract bool IsRunning();
		public abstract TestResult[] GetTestResults();

		public void SaveScreenShot()
		{
			((ITakesScreenshot) Driver).GetScreenshot().SaveAsFile(ScreenshotPath, ScreenshotFormat);
		}

		protected TestResult CreateTestResult(string testName, bool passed, string message)
		{
			return new TestResult
				{
					FileName = TestFilePath,
					TestName = testName,
					Passed = passed,
					Message = message,
					ScreenshotPath = ScreenshotPath
				};
		}
	}
}
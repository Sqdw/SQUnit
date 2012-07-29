using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace SQUnit
{
	public class JasmineTestSuite : TestSuiteBase
	{
		IWebElement _htmlReporter;

		public static readonly TestSuiteFactoryDelegate FactoryDelegate = (driver, path) => new JasmineTestSuite(driver, path);

		public JasmineTestSuite(IWebDriver driver, string testFilePath)
			: base(driver, testFilePath)
		{
		}

		public override void Update()
		{
			var elements = Driver.FindElements(By.CssSelector(".jasmine_reporter")).ToArray();
			if (elements.Length == 0)
			{
				SaveScreenShot();
				var msg = string.Format(
					"The test file is missing output of Jasmine HTML reporter - element '.jasmine_reporter' was not found. See [{0}] for details.",
					ScreenshotPath);
				throw new InvalidTestFileException(msg);
			}
			_htmlReporter = elements[0];
		}

		public override bool IsRunning()
		{
			return _htmlReporter.FindElements(By.CssSelector(".symbolSummary .pending")).Any();
		}

		public override TestResult[] GetTestResults()
		{
			if (IsRunning())
				return CreateTimeoutResult();

			var details = _htmlReporter
				.FindElements(By.CssSelector(".results .specDetail"))
				.Select(ParseSpecDetail)
				.ToArray();

			return _htmlReporter
				.FindElements(By.CssSelector(".results .specSummary"))
				.Select(e => ParseTestResult(e, details))
				.ToArray();
		}

		TestResult ParseTestResult(IWebElement specSummary, SpecDetail[] allDetails)
		{
			var description = specSummary.FindElement(By.CssSelector("a.description"));
			var id = description.GetAttribute("href");
			var testName = description.GetAttribute("title");
			var details = allDetails.FirstOrDefault(d => d.Id == id);
			var message = details != null ? details.Message : "(fail-message not found)";

			var resultClasses = ParseSpecClasses(specSummary).ToArray();

			if (resultClasses.Contains("passed"))
				return CreateTestResult(testName, true, string.Empty);

			if (resultClasses.Contains("failed"))
				return CreateTestResult(testName, false, message);

			return CreateTestResult(testName, false, "Unknown test class: '" + string.Join(" ", resultClasses) + "'");
		}

		static IEnumerable<string> ParseSpecClasses(IWebElement resultMessage)
		{
			return resultMessage
				.GetAttribute("class")
				.Split(' ')
				.Where(c => c != "resultMessage" && c != "specDetail" && c != "specSummary");
		}

		static SpecDetail ParseSpecDetail(IWebElement specDetail)
		{
			return new SpecDetail
			{
				Id = specDetail.FindElement(By.CssSelector("a.description")).GetAttribute("href"),
				Message = specDetail.FindElement(By.CssSelector(".resultMessage")).Text,
			};

		}

		class SpecDetail
		{
			public string Id { get; set; }
			public string Message { get; set; }
		}

		TestResult[] CreateTimeoutResult()
		{
			return new[]
				{
					CreateTestResult("(some tests)", false, "Some tests did not finish within time limit.")
				};
		}
	}
}
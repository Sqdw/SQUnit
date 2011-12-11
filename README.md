# Overview
SQUnit is a library that provides simple access to QUnit tests from any C# unit-testing framework. 

## Features
- The tests are run in a browser using Selenium WebDriver.
- After all unit-tests are run, a screenshot of the page is saved.
- Integration tests: you can generate HTML page dynamically using C# and run JavaScript unit-tests inside this page.

# Usage

```
// Create a TestRunner instance. 
// The instance can be reused to run multiple tests.
var runner = new TestRunner();

// Prepare the HTML file with QUnit scaffolding and 
// anything else you need to run your test.
var htmlFilePath = CreateTestFile();

// Run all QUnit tests in the HTML file
var testResults = runner.RunTestsInFile(htmlFilePath);

// Assert. SQUnit throws ApplicationException when some of the tests failed.
testResults.AssertAllTestsPassed();
```
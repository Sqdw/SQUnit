# Overview

SQUnit is an open-source library that provides a simple way to run JavaScript QUnit tests from any C# unit-testing framework.

## Features

- The tests are run in a browser using Selenium WebDriver.
- After all unit-tests are run, a screenshot of the page is saved.
- Integration tests: you can generate HTML page dynamically using C# and run JavaScript unit-tests inside this page.
- Performance - you can reuse single browser window for running multiple test suites.

## Development Status

We are considering the library as stable. It has been successfully used for running SQUnit tests for more than 6 months without any problems.

We have no new features planned at the moment. If you would like to have something added to the library, please fill an issue here on GitHub. We are open for proposals.

# Usage

## Installation

SQUnit is distributed via NuGet - see http://nuget.org/packages/SQUnit for more details.

## Writing unit-tests

```csharp
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

See [QUnit documentation](http://docs.jquery.com/QUnit) for information on JavaScript API and HTML scaffolding.

You can also look at test pages used for SQUnit tests: [SQUnit/Test/TestPages](https://github.com/Sqdw/SQUnit/tree/master/SQUnit/Test/TestPages)
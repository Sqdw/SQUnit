# Overview

SQUnit is an open-source library that provides a simple way to run JavaScript unit-tests from any C# unit-testing framework.

## Features

- Supports QUnit and Jasmine frameworks.
- The tests are run in a browser using Selenium WebDriver.
- After all unit-tests are run, a screenshot of the page is saved.
- Integration tests: you can generate HTML page dynamically using C# and run JavaScript unit-tests inside this page.
- Performance - you can reuse single browser window for running multiple test suites.

## Development Status

We are considering the library as stable. It has been successfully used for running QUnit tests for more than 6 months without any problems.

The Jasmine support was added recently and was not thouroughly tested yet.

We have no new features planned at the moment. If you would like to have something added to the library, please fill an issue here on GitHub. We are open for proposals.

# Usage

## Installation

SQUnit is distributed via NuGet - see http://nuget.org/packages/SQUnit for more details.

## Writing unit-tests - QUnit

```csharp
// Create a TestRunner instance.
// The instance can be reused to run multiple tests.
var runner = new TestRunner();
// Alternative way - specify QUnit explicitly
// var runner =  new TestRunner(QUnitTestSuite.FactoryDelegate);

// Prepare the HTML file with QUnit scaffolding and
// anything else you need to run your test.
var htmlFilePath = CreateTestFile();

// Run all QUnit tests in the HTML file
var testResults = runner.RunTestsInFile(htmlFilePath);

// Assert. SQUnit throws TestFailedException when some of the tests failed.
testResults.AssertAllTestsPassed();
```

See [QUnit documentation](http://docs.jquery.com/QUnit) for information on JavaScript API and HTML scaffolding.

You can also look at test pages used for SQUnit tests: [SQUnit/Test/QUnit](https://github.com/Sqdw/SQUnit/tree/master/SQUnit/Test/QUnit)

## Writing unit-tests - Jasmine

```csharp
// Create a TestRunner instance.
// The instance can be reused to run multiple tests.
var runner =  new TestRunner(JasmineTestSuite.FactoryDelegate);

// Prepare the HTML file with Jasmine scaffolding and
// anything else you need to run your test.
var htmlFilePath = CreateTestFile();

// Run all Jasmine tests in the HTML file
var testResults = runner.RunTestsInFile(htmlFilePath);

// Assert. SQUnit throws TestFailedException when some of the tests failed.
testResults.AssertAllTestsPassed();
```

See [Jasmine documentation](http://pivotal.github.com/jasmine/#section-The_Runner_and_Reporter) for information on JavaScript API and HTML scaffolding.

You can also look at the test runner used for SQUnit tests: [SQUnit/Test/Jasmine/runner.html](https://github.com/Sqdw/SQUnit/tree/master/SQUnit/Test/Jasmine/runner.html)

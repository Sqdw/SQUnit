namespace SQUnit
{
	public interface ITestSuite
	{
		void Update();
		bool IsRunning();
		TestResult[] GetTestResults();
		void SaveScreenShot();
	}
}
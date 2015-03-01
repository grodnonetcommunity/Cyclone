using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using AV.Cyclone.Katrina.Executor;
using NUnit.Framework;

namespace Test.Executor
{
    public class WeatherStationTest
    {
        [Test]
        public void SolutionTest()
        {
            var solutionPath = @"..\..\.TestSolution\TestSolution.sln";
            var filePath = @"..\..\.TestSolution\Test.Algorithms\BinarySerchTest.cs";
            var projectName = "Test.Algorithms";
            var solutionFullPath = GetFullFileName(solutionPath);
            var fileFullPath = GetFullFileName(filePath);

            var weatherStation = new WeatherStation(solutionFullPath, "Test.Algorithms", fileFullPath, 14);
            var manualResetEvent = new ManualResetEvent(false);
            weatherStation.Executed += (sender, args) => manualResetEvent.Set();
            weatherStation.Start();
            manualResetEvent.WaitOne();

            var operations = weatherStation.GetOperations(fileFullPath);
            Assert.NotNull(operations);
        }

        [Test]
        public void SolutionTestWithChanged()
        {
            var solutionPath = @"..\..\.TestSolution\TestSolution.sln";
            var filePath = @"..\..\.TestSolution\Test.Algorithms\BinarySerchTest.cs";
            var fileChangedPath = @"..\..\.TestSolution\Algorithms\BinarySearchChanged.cs";
            var projectName = "Test.Algorithms";
            var solutionFullPath = GetFullFileName(solutionPath);
            var fileFullPath = GetFullFileName(filePath);
            var fileChangedFullPath = GetFullFileName(fileChangedPath);

            var weatherStation = new WeatherStation(solutionFullPath, "Test.Algorithms", fileFullPath, 14);
            var resetEvent = new ManualResetEvent(false);
            weatherStation.Executed += (sender, args) => resetEvent.Set();
            weatherStation.Start();
            resetEvent.WaitOne();
            resetEvent.Reset();

            weatherStation.FileUpdated(fileFullPath, File.ReadAllText(fileChangedFullPath));
            resetEvent.WaitOne();

            var operations = weatherStation.GetOperations(fileFullPath);
            Assert.NotNull(operations);
        }

        public string GetFullFileName(string solutionPath, [CallerFilePath] string fileName = null)
        {
            var result = new StringBuilder();
            var combinedPath = Path.Combine(Path.GetDirectoryName(fileName), solutionPath);
            return Path.GetFullPath(combinedPath);
        }
    }
}
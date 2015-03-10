using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using AV.Cyclone.Katrina.Executor;
using AV.Cyclone.Sandy.Models;
using AV.Cyclone.Sandy.OperationParser;
using AV.Cyclone.Sandy.Tests;
using Microsoft.CodeAnalysis.MSBuild;

namespace AV.Cyclone.Sandy.UITests
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			var solutionPath = @"..\..\..\.TestSolution\TestSolution.sln";
			var projectName = "Test.Algorithms";
			var realSolutionPath = GetSolutionPath(solutionPath);

			var workspace = MSBuildWorkspace.Create();
			var solution = workspace.OpenSolutionAsync(realSolutionPath).Result.GetIsolatedSolution();

			var forecastExecutor = new ForecastExecutor(solution);
			forecastExecutor.SetStartupProject(projectName);
			var forecastItems = forecastExecutor.GetForecast();
			var files = forecastExecutor.GetReferences();

			var codeExecutor = new CodeExecutor();
			codeExecutor.Init(forecastItems);
			var executeLogger = new OperationsExecuteLogger();
			codeExecutor.SetExecuteLogger(executeLogger);
			codeExecutor.Execute(projectName, files, "Test.Algorithms.BinarySerchTest", "LessOrEqualRequired");

			InitializeComponent();
			List<Execution> executions = new List<Execution>();

			var t = executeLogger.MethodCalls.First().Value;

			foreach (var operationList in t)
			{
				//Simulate double
				executions.Add(new Execution
				{
					Operations = operationList
				});
				/*executions.Add(new Execution
				{
					Operations = operationList
				});*/
			}

			UIGenerator generator = new UIGenerator(executions);
			TextBlocks.Text = File.ReadAllText(@"..\..\..\..\..\.TestSolution\Algorithms\BinarySearch.cs");
			int numLines = TextBlocks.Text.Length - TextBlocks.Text.Replace(Environment.NewLine, string.Empty).Length;
			var components = generator.GetOutputComponents();
            for (int i = 0; i < numLines; i++)
			{
				UIElement element = components[i];

				
				if (element != null)
				{
					MainPanel.Children.Add(element);
				}
				else
				{
					MainPanel.Children.Add(new TextBlock());
				}
			}
		}

		public string GetSolutionPath(string solutionPath, [CallerFilePath] string fileName = null)
		{
			return Path.Combine(Path.GetDirectoryName(fileName), solutionPath);
		}
	}
}

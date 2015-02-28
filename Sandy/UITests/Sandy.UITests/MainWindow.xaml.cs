using System;
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
			var compilations = forecastExecutor.GetCompilations();
			var files = forecastExecutor.GetReferences();

			var codeExecutor = new CodeExecutor();
			codeExecutor.AddCompilations(compilations);
			var executeLogger = new OperationsExecuteLogger();
			codeExecutor.SetExecuteLogger(executeLogger);
			codeExecutor.Execute(projectName, files, "Test.Algorithms.BinarySerchTest", "LessOrEqualRequired");

			InitializeComponent();
			Execution execution = new Execution();

			var t = executeLogger.MethodCalls.First().Value;

			execution.Operations = t.First();

			UIGenerator generator = new UIGenerator(execution);
			TextBlocks.Text = File.ReadAllText("D:\\Projects\\GrandHackathon2015\\.TestSolution\\Algorithms\\BinarySearch.cs");
			int numLines = TextBlocks.Text.Length - TextBlocks.Text.Replace(Environment.NewLine, string.Empty).Length;
            for (int i = 0; i < numLines; i++)
			{
				UIElement element = generator.GetLine(i, "D:\\Projects\\GrandHackathon2015\\.TestSolution\\Algorithms\\BinarySearch.cs");

				
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

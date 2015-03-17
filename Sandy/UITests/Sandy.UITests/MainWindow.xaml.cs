using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using AV.Cyclone.Katrina.Executor;
using AV.Cyclone.Sandy.Models;
using AV.Cyclone.Sandy.OperationParser;
using Microsoft.CodeAnalysis.MSBuild;

namespace AV.Cyclone.Sandy.UITests
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            var solutionFolderPath = GetSolutionPath(@"..\..\..\.TestSolution");
            var solutionPath = Path.Combine(solutionFolderPath, "TestSolution.sln");
            var relativeFilePath = @"Test.Algorithms\BinarySerchTest.cs";
            var filePath = Path.Combine(solutionFolderPath, relativeFilePath);
            var projectName = "Test.Algorithms";

            var workspace = MSBuildWorkspace.Create();
            var solution = workspace.OpenSolutionAsync(solutionPath).Result.GetIsolatedSolution();

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
            var executions = new List<Execution>(executeLogger.MethodCalls
                .Where(mr => mr.Key.FileName.EndsWith(relativeFilePath))
                .SelectMany(e => e.Value)
                .Select(e => new Execution {Operations = e}));

            var executeTree = ExecuteTree.Generate(executions[0].Operations);
            var generator = new UiGenerator2();
            generator.Generate(executeTree);
            
            var fileContent = File.ReadAllText(filePath);
            var lines = Regex.Split(fileContent, Environment.NewLine);
            int numLines = lines.Length;
            var components = generator.GetOutputComponents();
            for (int i = 0; i < numLines; i++)
            {
                mainGrid.RowDefinitions.Add(new RowDefinition {Height = GridLength.Auto});

                var codeLine = CreateTextBlock(lines[i]);
                var element = components[i];

                AddControl(codeLine, i, 0);
                if (element != null) AddControl(element, i, 1);
            }
        }

        private void AddControl(UIElement element, int row, int column)
        {
            Grid.SetRow(element, row);
            Grid.SetColumn(element, column);

            mainGrid.Children.Add(element);
        }

        private static TextBlock CreateTextBlock(string text)
        {
            var codeLine = new TextBlock(new Run(text))
            {
                FontFamily = new FontFamily("Consolas"),
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center
            };
            return codeLine;
        }

        public string GetSolutionPath(string path, [CallerFilePath] string fileName = null)
        {
            var combinedPath = Path.Combine(Path.GetDirectoryName(fileName), path);
            return Path.GetFullPath(combinedPath);
        }
    }
}

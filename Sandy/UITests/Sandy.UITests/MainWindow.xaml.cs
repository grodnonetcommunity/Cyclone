﻿using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using AV.Cyclone.Sandy.OperationParser;
using AV.Cyclone.Sandy.Tests;

namespace AV.Cyclone.Sandy.UITests
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			ModelTestClass modelTestClass = new ModelTestClass();
			modelTestClass.Init();

			var execution = modelTestClass.Execution;
			UIGenerator generator = new UIGenerator(execution);
			for (int i = 0; i < 20; i++)
			{
				UIElement element = generator.GetLine(i, "1");
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
	}
}

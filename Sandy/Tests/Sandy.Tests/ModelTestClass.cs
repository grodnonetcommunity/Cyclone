using System.Collections.Generic;
using AV.Cyclone.Sandy.Models;
using AV.Cyclone.Sandy.Models.Operations;
using AV.Cyclone.Sandy.OperationParser;
using NUnit.Framework;

namespace AV.Cyclone.Sandy.Tests
{
	[TestFixture]
	public class ModelTestClass
	{
		public Execution Execution { get; set; }

	    public ModelTestClass()
	    {
	        Execution = new Execution();
	    }

	    [TestFixtureSetUp]
		public void Init()
		{
			//The main List of operations goes here
			List<Operation> operations = new List<Operation>();
			Execution.Operations = operations;

			AssignOperation assignTestArray = new AssignOperation
			{
				FileName = "1",
				LineNumber = 8,
				VariableName = "testarray",
				VariableValue = new[] { 1, 2, 5, 3 }
			};

			operations.Add(assignTestArray);

			AssignOperation assignLow = new AssignOperation
			{
				FileName = "1",
				LineNumber = 10,
				VariableName = "low",
				VariableValue = 0
			};

			AssignOperation assignHigh = new AssignOperation
			{
				FileName = "1",
				LineNumber = 10,
				VariableName = "high",
				VariableValue = 5
			};

			operations.Add(assignLow);
			operations.Add(assignHigh);

			AssignOperation assignMid = new AssignOperation
			{
				FileName = "1",
				LineNumber = 12,
				VariableName = "mid",
				VariableValue = 2
			};

			operations.Add(assignMid);

			LoopOperation loopOperation = new LoopOperation
			{
				FileName = "1",
				LineNumber = 13
			};
			//Iteration 1
			AssignOperation assignLow1 = new AssignOperation()
			{
				FileName = "1",
				LineNumber = 15,
				VariableName = "low",
				VariableValue = 2
			};

			loopOperation.Operations.Add(0, new List<Operation> { assignLow1 });

			//Iteration 2 
			//Imagine that all values change
			AssignOperation assignLow2 = new AssignOperation()
			{
				FileName = "1",
				LineNumber = 15,
				VariableName = "low",
				VariableValue = 3
			};

			AssignOperation assignMid2 = new AssignOperation()
			{
				FileName = "1",
				LineNumber = 14,
				VariableName = "mid",
				VariableValue = 2
			};

			AssignOperation assignHigh2 = new AssignOperation()
			{
				FileName = "1",
				LineNumber = 16,
				VariableName = "high",
				VariableValue = 16
			};

			loopOperation.Operations.Add(1, new List<Operation>
			{
				assignLow2,
				assignMid2,
				assignHigh2
			});

			//Iteration 3
			//2 values change
			AssignOperation assignMid3 = new AssignOperation()
			{
				FileName = "1",
				LineNumber = 14,
				VariableName = "mid",
				VariableValue = 3
			};

			AssignOperation assignHigh3 = new AssignOperation()
			{
				FileName = "1",
				LineNumber = 16,
				VariableName = "high",
				VariableValue = 4
			};

			loopOperation.Operations.Add(2, new List<Operation>
			{
				assignMid3, assignHigh3
			});

			operations.Add(loopOperation);
		}

		[Test]
		public void OperationFinderTest()
		{
			UIGenerator generator = new UIGenerator(this.Execution);
			List<Operation> resultFirst = new List<Operation>();
			generator.SearchOperation(14, Execution.Operations, resultFirst);
			Assert.AreEqual(resultFirst.Count, 2);

			List<Operation> resultSecond = new List<Operation>();
			generator.SearchOperation(10, Execution.Operations, resultSecond);
			Assert.AreEqual(resultSecond.Count, 2);
		}

		[Test, RequiresSTA]
		public void TotalComponentTest()
		{
			UIGenerator generator = new UIGenerator(this.Execution);
			var components = generator.GetOutputComponents();
			Assert.AreEqual(components.Count, 7);
		}
	}
}
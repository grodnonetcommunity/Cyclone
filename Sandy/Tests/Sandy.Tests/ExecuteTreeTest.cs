using System.Collections.Generic;
using System.Linq;
using AV.Cyclone.Sandy.Models.Operations;
using AV.Cyclone.Sandy.OperationParser;
using NUnit.Framework;

namespace AV.Cyclone.Sandy.Tests
{
    public class ExecuteTreeTest
    {
        [Test]
        public void AssignOperationTest()
        {
            var executionTree = new ExecuteTree("Test");

            executionTree.Add(new AssignOperation{LineNumber = 0, VariableName = "x", VariableValue = 0});
            executionTree.Add(new AssignOperation{LineNumber = 1, VariableName = "y", VariableValue = 0});
            executionTree.Add(new AssignOperation{LineNumber = 2, VariableName = "z", VariableValue = 0});

            Assert.AreEqual(3, executionTree.Lines.Count);
        }

        [Test]
        public void MergeTest()
        {
            var executionTree1 = new ExecuteTree("Test");

            executionTree1.Add(new AssignOperation{LineNumber = 0, VariableName = "x", VariableValue = 100});
            executionTree1.Add(new AssignOperation{LineNumber = 1, VariableName = "y", VariableValue = 200});
            executionTree1.Add(new AssignOperation{LineNumber = 2, VariableName = "z", VariableValue = 300});

            var executionTree2 = new ExecuteTree("Test");

            executionTree2.Add(new AssignOperation{LineNumber = 0, VariableName = "x", VariableValue = 101});
            executionTree2.Add(new AssignOperation{LineNumber = 1, VariableName = "y", VariableValue = 201});
            executionTree2.Add(new AssignOperation{LineNumber = 2, VariableName = "z", VariableValue = 301});

            var executionTree = new ExecuteTree("Test");

            executionTree.Add(new[] {executionTree1, executionTree2});

            Assert.AreEqual(3, executionTree.Lines.Count);

            /*Assert.That(executionTree.Lines[0].Executions, Has.Count.EqualTo(1).And.All.InstanceOf<ListExecuteTreeLineItem>());
            Assert.That(executionTree.Lines[1].Executions, Has.Count.EqualTo(1).And.All.InstanceOf<ListExecuteTreeLineItem>());
            Assert.That(executionTree.Lines[2].Executions, Has.Count.EqualTo(1).And.All.InstanceOf<ListExecuteTreeLineItem>());*/

            /*Assert.That(((ListExecuteTreeLineItem)executionTree.Lines[0].Executions[0]).Items.Select(e => e.Value).ToList(),
                        Has.Count.EqualTo(2).And.All.InstanceOf<AssignOperationExecuteTreeLineItem>());
            Assert.That(((ListExecuteTreeLineItem)executionTree.Lines[1].Executions[0]).Items.Select(e => e.Value).ToList(),
                        Has.Count.EqualTo(2).And.All.InstanceOf<AssignOperationExecuteTreeLineItem>());
            Assert.That(((ListExecuteTreeLineItem)executionTree.Lines[2].Executions[0]).Items.Select(e => e.Value).ToList(),
                        Has.Count.EqualTo(2).And.All.InstanceOf<AssignOperationExecuteTreeLineItem>());*/
        }

        [Test]
        public void ComplexMergeTest()
        {
            var executionTree1 = new ExecuteTree("Test");

            executionTree1.Add(new AssignOperation{LineNumber = 2, VariableName = "x", VariableValue = 100});
            executionTree1.Add(new AssignOperation{LineNumber = 4, VariableName = "z", VariableValue = 300});

            var executionTree2 = new ExecuteTree("Test");

            executionTree2.Add(new AssignOperation{LineNumber = 3, VariableName = "y", VariableValue = 201});

            var executionTree = new ExecuteTree("Test");

            executionTree.Add(new AssignOperation { LineNumber = 0, VariableName = "a", VariableValue = 50 });
            executionTree.Add(new AssignOperation { LineNumber = 1, VariableName = "b", VariableValue = 25 });
            executionTree.Add(new[] { executionTree1, executionTree2 });

            Assert.AreEqual(5, executionTree.Lines.Count);
        }

        [Test]
        public void ComplexMergeTest2()
        {
            var executionTree1 = new ExecuteTree("Test");

            executionTree1.Add(new AssignOperation{LineNumber = 2, VariableName = "x", VariableValue = 100});

            var executionTree2 = new ExecuteTree("Test");

            executionTree2.Add(new AssignOperation{LineNumber = 3, VariableName = "y", VariableValue = 201});

            var executionTree3 = new ExecuteTree("Test");

            executionTree1.Add(new AssignOperation { LineNumber = 2, VariableName = "x", VariableValue = 300 });

            var executionTree = new ExecuteTree("Test");

            executionTree.Add(new AssignOperation { LineNumber = 0, VariableName = "a", VariableValue = 50 });
            executionTree.Add(new AssignOperation { LineNumber = 1, VariableName = "b", VariableValue = 25 });
            executionTree.Add(new[] { executionTree1, executionTree2, executionTree3 });

            Assert.AreEqual(4, executionTree.Lines.Count);
        }

        [Test]
        public void GenerateTest()
        {
            var operations = new List<Operation>();

            operations.Add(new AssignOperation{LineNumber = 0, VariableName = "a", VariableValue = 50});
            operations.Add(new AssignOperation{LineNumber = 1, VariableName = "b", VariableValue = 25});

            var loopOperation = new LoopOperation();

            loopOperation.Operations[0] = new List<Operation>
                                          {
                                              new AssignOperation{LineNumber = 2, VariableName = "x", VariableValue = 10}
                                          };
            loopOperation.Operations[1] = new List<Operation>
                                          {
                                              new AssignOperation{LineNumber = 3, VariableName = "y", VariableValue = 20}
                                          };
            loopOperation.Operations[2] = new List<Operation>
                                          {
                                              new AssignOperation{LineNumber = 2, VariableName = "x", VariableValue = 30}
                                          };

            operations.Add(loopOperation);

            var executionTree = ExecuteTree.Generate("Test", operations);

            Assert.AreEqual(4, executionTree.Lines.Count);
        }

        [Test]
        public void TwoVariablesOnTheSameLineTest()
        {
            var operations1 = new List<Operation>
                              {
                                  new AssignOperation {LineNumber = 0, VariableName = "x", VariableValue = 1},
                                  new AssignOperation {LineNumber = 0, VariableName = "y", VariableValue = 1},
                              };

            var executionTree1 = ExecuteTree.Generate("Test", operations1);

            var executionTree = new ExecuteTree("Test");
            executionTree.Add(new [] {executionTree1, executionTree1, });
        }
    }
}
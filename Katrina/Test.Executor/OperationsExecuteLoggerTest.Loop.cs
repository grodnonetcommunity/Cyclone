using System.Collections.Generic;
using System.Linq;
using AV.Cyclone.Katrina.Executor;
using AV.Cyclone.Sandy.Models.Operations;
using NUnit.Framework;

namespace Test.Executor
{
    public partial class OperationsExecuteLoggerTest
    {
        [Test]
        public void SingleLoopTest()
        {
            var opertaionsExecuteLogger = new OperationsExecuteLogger();
            opertaionsExecuteLogger.BeginMethod("M", "", 0);
            {
                opertaionsExecuteLogger.LogAssign("a", "", 1, 1);
                opertaionsExecuteLogger.BeginLoop("", 2);
                {
                    // Iteration 1
                    opertaionsExecuteLogger.LoopIteration("", 3);
                    opertaionsExecuteLogger.LogAssign("b", "", 4, 1);
                    // Iteration 2
                    opertaionsExecuteLogger.LoopIteration("", 3);
                    opertaionsExecuteLogger.LogAssign("b", "", 4, 2);
                }
                opertaionsExecuteLogger.EndLoop("", 2);
            }
            opertaionsExecuteLogger.EndMethod("M", "", 0);

            Assert.AreEqual(1, opertaionsExecuteLogger.MethodCalls.Count); // Single method was called
            var firstMethod = opertaionsExecuteLogger.MethodCalls.Values.Skip(0).First();
            Assert.AreEqual(1, firstMethod.Count); // With 1 call
            Assert.AreEqual(2, firstMethod[0].Count); // with two operations in fir call

            //Assert.AreEqual(1, opertaionsExecuteLogger.MethodCalls.Values.Skip(1).First().Count);
            //Assert.AreEqual(1, opertaionsExecuteLogger.MethodCalls.Values.Skip(1).First()[0].Count);
            //Assert.AreEqual(1, opertaionsExecuteLogger.MethodCalls.Values.Skip(2).First().Count);
            //Assert.AreEqual(1, opertaionsExecuteLogger.MethodCalls.Values.Skip(2).First()[0].Count);
        }
    }
}
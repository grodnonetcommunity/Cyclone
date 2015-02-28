using System.Linq;
using AV.Cyclone.Katrina.Executor;
using NUnit.Framework;

namespace Test.Executor
{
    public partial class OperationsExecuteLoggerTest
    {
        [Test]
        public void MethodCallTest()
        {
            var opertaionsExecuteLogger = new OperationsExecuteLogger();
            opertaionsExecuteLogger.BeginMethod("M", "", 0);
            opertaionsExecuteLogger.LogAssign("i", "", 1, 0);
            opertaionsExecuteLogger.EndMethod("M", "", 0);

            Assert.AreEqual(1, opertaionsExecuteLogger.MethodCalls.Count); // Only one method was called
            Assert.AreEqual(1, opertaionsExecuteLogger.MethodCalls.Values.First().Count); // And was called only once
            Assert.AreEqual(1, opertaionsExecuteLogger.MethodCalls.Values.First()[0].Count); // with one operation
        }

        [Test]
        public void InnerCallTest()
        {
            var opertaionsExecuteLogger = new OperationsExecuteLogger();
            opertaionsExecuteLogger.BeginMethod("M1", "", 0);
            {
                opertaionsExecuteLogger.LogAssign("i", "", 1, 1);
                opertaionsExecuteLogger.BeginMethod("M2", "", 2);
                {
                    opertaionsExecuteLogger.LogAssign("j", "", 1, 1);
                }
                opertaionsExecuteLogger.EndMethod("M2", "", 0);
            }
            opertaionsExecuteLogger.EndMethod("M1", "", 0);

            Assert.AreEqual(2, opertaionsExecuteLogger.MethodCalls.Count); // Two methods was called
            Assert.AreEqual(1, opertaionsExecuteLogger.MethodCalls.Values.First().Count); // And each called only once
            Assert.AreEqual(1, opertaionsExecuteLogger.MethodCalls.Values.First()[0].Count); // with one operation each
            Assert.AreEqual(1, opertaionsExecuteLogger.MethodCalls.Values.Last().Count);
            Assert.AreEqual(1, opertaionsExecuteLogger.MethodCalls.Values.Last()[0].Count);
        }

        [Test]
        public void InnerCallTest2()
        {
            var opertaionsExecuteLogger = new OperationsExecuteLogger();
            opertaionsExecuteLogger.BeginMethod("M1", "", 0);
            {
                opertaionsExecuteLogger.LogAssign("i", "", 1, 1);
                opertaionsExecuteLogger.BeginMethod("M2", "", 2);
                {
                    opertaionsExecuteLogger.LogAssign("j", "", 1, 1);
                    opertaionsExecuteLogger.BeginMethod("M3", "", 2);
                    {
                        opertaionsExecuteLogger.LogAssign("k", "", 1, 1);
                    }
                    opertaionsExecuteLogger.EndMethod("M3", "", 0);
                }
                opertaionsExecuteLogger.EndMethod("M2", "", 0);
            }
            opertaionsExecuteLogger.EndMethod("M1", "", 0);

            Assert.AreEqual(3, opertaionsExecuteLogger.MethodCalls.Count); // Three methods was called
            Assert.AreEqual(1, opertaionsExecuteLogger.MethodCalls.Values.Skip(0).First().Count); // And each called only once
            Assert.AreEqual(1, opertaionsExecuteLogger.MethodCalls.Values.Skip(0).First()[0].Count); // with one operation each
            Assert.AreEqual(1, opertaionsExecuteLogger.MethodCalls.Values.Skip(1).First().Count);
            Assert.AreEqual(1, opertaionsExecuteLogger.MethodCalls.Values.Skip(1).First()[0].Count);
            Assert.AreEqual(1, opertaionsExecuteLogger.MethodCalls.Values.Skip(2).First().Count);
            Assert.AreEqual(1, opertaionsExecuteLogger.MethodCalls.Values.Skip(2).First()[0].Count);
        }

        [Test]
        public void SeparateInnerCallTest2()
        {
            var opertaionsExecuteLogger = new OperationsExecuteLogger();
            opertaionsExecuteLogger.BeginMethod("M1", "", 0);
            {
                opertaionsExecuteLogger.LogAssign("i", "", 1, 1);
                opertaionsExecuteLogger.BeginMethod("M2", "", 2);
                {
                    opertaionsExecuteLogger.LogAssign("j", "", 1, 1);
                }
                opertaionsExecuteLogger.EndMethod("M2", "", 0);
                opertaionsExecuteLogger.BeginMethod("M3", "", 2);
                { 
                    opertaionsExecuteLogger.LogAssign("k", "", 1, 1);
                }
                opertaionsExecuteLogger.EndMethod("M3", "", 0);
            }
            opertaionsExecuteLogger.EndMethod("M1", "", 0);

            Assert.AreEqual(3, opertaionsExecuteLogger.MethodCalls.Count); // Three methods was called
            Assert.AreEqual(1, opertaionsExecuteLogger.MethodCalls.Values.Skip(0).First().Count); // And each called only once
            Assert.AreEqual(1, opertaionsExecuteLogger.MethodCalls.Values.Skip(0).First()[0].Count); // with one operation each
            Assert.AreEqual(1, opertaionsExecuteLogger.MethodCalls.Values.Skip(1).First().Count);
            Assert.AreEqual(1, opertaionsExecuteLogger.MethodCalls.Values.Skip(1).First()[0].Count);
            Assert.AreEqual(1, opertaionsExecuteLogger.MethodCalls.Values.Skip(2).First().Count);
            Assert.AreEqual(1, opertaionsExecuteLogger.MethodCalls.Values.Skip(2).First()[0].Count);
        }
    }
}
using AV.Cyclone.Katrina.Executor;
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
        }
    }
}
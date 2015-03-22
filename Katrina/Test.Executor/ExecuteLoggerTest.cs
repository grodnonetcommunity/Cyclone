using AV.Cyclone.Katrina.Executor;
using AV.Cyclone.Katrina.Executor.Interfaces;
using NUnit.Framework;

namespace Test.Executor
{
    public class ExecuteLoggerTest
    {
        [Test]
        public void SimpleTest()
        {
            var executorLogger = new BaseExecuteLoggerHelper();
            Context.ExecuteLoggerHelper = executorLogger;

            var intValue = Context.ExecuteLoggerHelper.LogAssign("intValue", "ExecuteLoggerTest.cs", 14, 42);
            var stringValue = Context.ExecuteLoggerHelper.LogAssign("stringValue", "ExecuteLoggerTest.cs", 15, "String");
            var nullValue = Context.ExecuteLoggerHelper.LogAssign("nullValue", "ExecuteLoggerTest.cs", 16, (object)null);

            Assert.AreEqual(42, intValue);
            Assert.AreEqual("String", stringValue);
            Assert.IsNull(nullValue);
        }
    }
}

using AV.Cyclone.Katrina.Executor;
using NUnit.Framework;

namespace Test.Executor
{
    public class ExecuteLoggerTest
    {
        [Test]
        public void SimpleTest()
        {
            var executorLogger = new BaseExecuteLogger();
            Context.ExecuteLogger = executorLogger;

            var intValue = Context.ExecuteLogger.LogAssign("intValue", "ExecuteLoggerTest.cs", 14, 42);
            var stringValue = Context.ExecuteLogger.LogAssign("stringValue", "ExecuteLoggerTest.cs", 15, "String");
            var nullValue = Context.ExecuteLogger.LogAssign("nullValue", "ExecuteLoggerTest.cs", 16, (object)null);

            Assert.AreEqual(42, intValue);
            Assert.AreEqual("String", stringValue);
            Assert.IsNull(nullValue);
        }
    }
}

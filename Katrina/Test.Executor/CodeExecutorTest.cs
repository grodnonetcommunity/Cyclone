using System.Diagnostics;
using System.Text;
using AV.Cyclone.Katrina.Executor;
using AV.Cyclone.Katrina.Executor.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

namespace Test.Executor
{
    public class CodeExecutorTest
    {
        public static MetadataReference Mscorelib
        {
            get
            {
                return MetadataReference.CreateFromFile(typeof (object).Assembly.Location);
            }
        }

        public static MetadataReference ExecutorInterfaces
        {
            get
            {
                return MetadataReference.CreateFromFile(typeof(Context).Assembly.Location);
            }
        }

        [Test]
        public void SimpleTest()
        {
            var source = @"
class Class
{
    public void Method()
    {
        var a = AV.Cyclone.Katrina.Executor.Interfaces.Context.ExecuteLoggerHelper.LogAssign(""a"","""",4,1);
        var b = AV.Cyclone.Katrina.Executor.Interfaces.Context.ExecuteLoggerHelper.LogAssign(""b"","""",4,1);
    }
}
";
            var syntaxTree = CSharpSyntaxTree.ParseText(source);

            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var compilaton = CSharpCompilation.Create("Temp.dll", new[] {syntaxTree,}, new[] {Mscorelib, ExecutorInterfaces, },
                compilationOptions);

            var executor = new CodeExecutor();
            var executeLogger = new MockExecuteLogger();

            executor.Init(new[]
            {
                new ForecastItem
                {
                    Compilation = compilaton,
                    SyntaxTree = syntaxTree
                }
            });
            executor.SetExecuteLogger(executeLogger);
            executor.Execute(compilaton.AssemblyName, null, "Class", "Method");

            CollectionAssert.AreEqual(new [] {"a = 1", "b = 1"}, executeLogger.assigns);
        }

        [Test]
        public void ContinuesRunTest()
        {
            var source1 = @"
class Class
{
    public void Method()
    {
        var a = AV.Cyclone.Katrina.Executor.Interfaces.Context.ExecuteLoggerHelper.LogAssign(""a"","""",4,1);
    }
}
";
            var source2 = @"
class Class
{
    public void Method()
    {
        var a = AV.Cyclone.Katrina.Executor.Interfaces.Context.ExecuteLoggerHelper.LogAssign(""a"","""",4,1);
        var b = AV.Cyclone.Katrina.Executor.Interfaces.Context.ExecuteLoggerHelper.LogAssign(""b"","""",4,1);
    }
}
";
            var syntaxTree1 = CSharpSyntaxTree.ParseText(source1);
            syntaxTree1 = CSharpSyntaxTree.Create((CSharpSyntaxNode)syntaxTree1.GetRoot(), path: "File1.cs",
                encoding: Encoding.UTF8);

            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var compilation1 = CSharpCompilation.Create("Temp.dll", new[] {syntaxTree1,}, new[] {Mscorelib, ExecutorInterfaces, },
                compilationOptions);

            var executor = new CodeExecutor();
            var executeLogger = new MockExecuteLogger();

            var stopwatch = Stopwatch.StartNew();
            executor.Init(new[]
            {
                new ForecastItem
                {
                    Compilation = compilation1,
                    SyntaxTree = syntaxTree1
                }
            });
            executor.SetExecuteLogger(executeLogger);
            executor.Execute(compilation1.AssemblyName, null, "Class", "Method");
            stopwatch.Stop();
            Debug.WriteLine("First execute in: {0} ms", stopwatch.ElapsedMilliseconds);

            CollectionAssert.AreEqual(new [] {"a = 1"}, executeLogger.assigns);

            executeLogger.assigns.Clear();

            stopwatch.Restart();
            executor.UpdateFile("File1.cs", source2);
            executor.Execute(compilation1.AssemblyName, null, "Class", "Method");
            Debug.WriteLine("Second execute in: {0} ms", stopwatch.ElapsedMilliseconds);

            CollectionAssert.AreEqual(new [] {"a = 1", "b = 1"}, executeLogger.assigns);
        }

        [Test]
        public void ContinuesRunTest2()
        {
            var source1 = @"
class Class
{
    public void Method()
    {
        var a = AV.Cyclone.Katrina.Executor.Interfaces.Context.ExecuteLoggerHelper.LogAssign(""a"","""",4,1);
    }
}
";
            var source2 = @"
class Class
{
    public void Method()
    {
        var a = AV.Cyclone.Katrina.Executor.Interfaces.Context.ExecuteLoggerHelper.LogAssign(""a"","""",4,1);
        var b = AV.Cyclone.Katrina.Executor.Interfaces.Context.ExecuteLoggerHelper.LogAssign(""b"","""",4,1);
    }
}
";
            var syntaxTree1 = CSharpSyntaxTree.ParseText(source1);
            syntaxTree1 = CSharpSyntaxTree.Create((CSharpSyntaxNode)syntaxTree1.GetRoot(), path: "File1.cs",
                encoding: Encoding.UTF8);

            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var compilation1 = CSharpCompilation.Create("Temp.dll", new[] {syntaxTree1,}, new[] {Mscorelib, ExecutorInterfaces, },
                compilationOptions);

            var executor = new CodeExecutor();
            var executeLogger = new MockExecuteLogger();

            var stopwatch = Stopwatch.StartNew();
            executor.Init(new[]
            {
                new ForecastItem
                {
                    Compilation = compilation1,
                    SyntaxTree = syntaxTree1
                }
            });
            executor.SetExecuteLogger(executeLogger);
            executor.Execute(compilation1.AssemblyName, null, "Class", "Method");
            stopwatch.Stop();
            Debug.WriteLine("First execute in: {0} ms", stopwatch.ElapsedMilliseconds);

            CollectionAssert.AreEqual(new [] {"a = 1"}, executeLogger.assigns);

            executeLogger.assigns.Clear();

            stopwatch.Restart();
            executor.UpdateFile("File1.cs", source2);
            executor.Execute(compilation1.AssemblyName, null, "Class", "Method");
            Debug.WriteLine("Second execute in: {0} ms", stopwatch.ElapsedMilliseconds);

            CollectionAssert.AreEqual(new [] {"a = 1", "b = 1"}, executeLogger.assigns);

            executeLogger.assigns.Clear();

            stopwatch.Restart();
            executor.UpdateFile("File1.cs", source1);
            executor.Execute(compilation1.AssemblyName, null, "Class", "Method");
            Debug.WriteLine("Second execute in: {0} ms", stopwatch.ElapsedMilliseconds);

            CollectionAssert.AreEqual(new [] {"a = 1"}, executeLogger.assigns);
        }

        [Test]
        public void ComplexTypeTest()
        {
            var source = @"
class Class
{
    class TestClass
    {
        public int x;
        public int y;
    }

    public void Method()
    {
        var a = AV.Cyclone.Katrina.Executor.Interfaces.Context.ExecuteLoggerHelper.LogAssign(""a"","""",4,1);
        var b = AV.Cyclone.Katrina.Executor.Interfaces.Context.ExecuteLoggerHelper.LogAssign(""b"","""",4,1);
        var c = AV.Cyclone.Katrina.Executor.Interfaces.Context.ExecuteLoggerHelper.LogAssign(""c"","""",4, new TestClass());
    }
}
";
            var syntaxTree = CSharpSyntaxTree.ParseText(source);

            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var compilaton = CSharpCompilation.Create("Temp.dll", new[] { syntaxTree, }, new[] { Mscorelib, ExecutorInterfaces, },
                compilationOptions);

            var executor = new CodeExecutor();
            var executeLogger = new MockExecuteLogger();

            executor.Init(new[]
            {
                new ForecastItem
                {
                    Compilation = compilaton,
                    SyntaxTree = syntaxTree
                }
            });
            executor.SetExecuteLogger(executeLogger);
            executor.Execute(compilaton.AssemblyName, null, "Class", "Method");

            CollectionAssert.AreEqual(new[] { "a = 1", "b = 1", "c = Class+TestClass" }, executeLogger.assigns);
        }

        [Test]
        public void ArraySerializeTest()
        {
            var source = @"
class Class
{
    public void Method()
    {
        var a = AV.Cyclone.Katrina.Executor.Interfaces.Context.ExecuteLoggerHelper.LogAssign(""a"","""",4, new int[] {1, 2, 3, 4});
    }
}
";
            var syntaxTree = CSharpSyntaxTree.ParseText(source);

            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var compilaton = CSharpCompilation.Create("Temp.dll", new[] { syntaxTree, }, new[] { Mscorelib, ExecutorInterfaces, },
                compilationOptions);

            var executor = new CodeExecutor();
            var executeLogger = new MockExecuteLogger();

            executor.Init(new[]
            {
                new ForecastItem
                {
                    Compilation = compilaton,
                    SyntaxTree = syntaxTree
                }
            });
            executor.SetExecuteLogger(executeLogger);
            executor.Execute(compilaton.AssemblyName, null, "Class", "Method");

            Assert.AreEqual(new[] {1, 2, 3, 4}, executeLogger.values[0]);
        }
    }
}
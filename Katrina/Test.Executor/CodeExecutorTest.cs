using System.Diagnostics;
using AV.Cyclone.Katrina.Executor;
using AV.Cyclone.Katrina.Executor.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

namespace Test.Executor
{
    public class CodeExecutorTest
    {
        public static MetadataReference Mscorelib => MetadataReference.CreateFromAssembly(typeof (object).Assembly);

        public static MetadataReference ExecutorInterfaces => MetadataReference.CreateFromAssembly(typeof (Context).Assembly);

        [Test]
        public void SimpleTest()
        {
            var source = @"
class Class
{
    public void Method()
    {
        var a = AV.Cyclone.Katrina.Executor.Interfaces.Context.ExecuteLogger.LogAssign(""a"","""",4,1);
        var b = AV.Cyclone.Katrina.Executor.Interfaces.Context.ExecuteLogger.LogAssign(""b"","""",4,1);
    }
}
";
            var syntaxTree = CSharpSyntaxTree.ParseText(source);

            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var compilaton = CSharpCompilation.Create("Temp.dll", new[] {syntaxTree,}, new[] {Mscorelib, ExecutorInterfaces, },
                compilationOptions);

            var executor = new CodeExecutor();
            var executeLogger = new MockExecuteLogger();

            executor.AddCompilation(null, compilaton);
            executor.SetExecuteLogger(executeLogger);
            executor.Execute(compilaton.AssemblyName, "Class", "Method");

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
        var a = AV.Cyclone.Katrina.Executor.Interfaces.Context.ExecuteLogger.LogAssign(""a"","""",4,1);
    }
}
";
            var source2 = @"
class Class
{
    public void Method()
    {
        var a = AV.Cyclone.Katrina.Executor.Interfaces.Context.ExecuteLogger.LogAssign(""a"","""",4,1);
        var b = AV.Cyclone.Katrina.Executor.Interfaces.Context.ExecuteLogger.LogAssign(""b"","""",4,1);
    }
}
";
            var syntaxTree1 = CSharpSyntaxTree.ParseText(source1);

            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var compilation1 = CSharpCompilation.Create("Temp.dll", new[] {syntaxTree1,}, new[] {Mscorelib, ExecutorInterfaces, },
                compilationOptions);

            var executor = new CodeExecutor();
            var executeLogger = new MockExecuteLogger();

            var stopwatch = Stopwatch.StartNew();
            executor.AddCompilation(null, compilation1);
            executor.SetExecuteLogger(executeLogger);
            executor.Execute(compilation1.AssemblyName, "Class", "Method");
            stopwatch.Stop();
            Debug.WriteLine("First execute in: {0} ms", stopwatch.ElapsedMilliseconds);

            CollectionAssert.AreEqual(new [] {"a = 1"}, executeLogger.assigns);

            executeLogger.assigns.Clear();

            var syntaxTree2 = CSharpSyntaxTree.ParseText(source2);

            var compilation2 = compilation1.ReplaceSyntaxTree(syntaxTree1, syntaxTree2);

            stopwatch.Restart();
            executor.AddCompilation(compilation1, compilation2);
            executor.Execute(compilation2.AssemblyName, "Class", "Method");
            Debug.WriteLine("Second execute in: {0} ms", stopwatch.ElapsedMilliseconds);

            CollectionAssert.AreEqual(new [] {"a = 1", "b = 1"}, executeLogger.assigns);
        }
    }
}
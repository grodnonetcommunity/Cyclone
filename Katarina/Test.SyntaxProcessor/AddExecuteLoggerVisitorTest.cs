using AV.Cyclone.Katrina.SyntaxProcessor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace Test.SyntaxProcessor
{
    public class AddExecuteLoggerVisitorTest
    {
        [Test]
        public void CreateMemberAccessTest()
        {
            var memberAccess = "AV.Cyclone.Katrina.Executor.Context.ExecuteLogger.LogAssign";
            var memberAccessExpression = AddExecuteLoggerVisitor.CreateMemberAccess(memberAccess);
            Assert.AreEqual(memberAccess, memberAccessExpression.ToString());
        }

        [Test]
        public void VariableDeclaratorTest()
        {
            var source = "var a = 0";
            var tree = ParseMethodBody(source);

            var visitor = CreateAddExecuteLoggerVisitor();
            var newTree = visitor.Visit(tree);

            AreEqualCode(@"var a = LA(""a"","""",0,0);", newTree);
        }

        [Test]
        public void VariableAssignTest()
        {
            var source = "a = 0";
            var tree = ParseMethodBody(source);

            var visitor = CreateAddExecuteLoggerVisitor();
            var newTree = visitor.Visit(tree);

            AreEqualCode(@"a = LA(""a"","""",0,0);", newTree);
        }

        [Test]
        public void WhileLoopTest()
        {
            var source = "while (true) {}";
            var tree = ParseMethodBody(source);

            var visitor = CreateAddExecuteLoggerVisitor();
            var newTree = visitor.Visit(tree);

            var expected = @"
    try
    {
        BL("""", 0);
        while (true) {}
    }
    finally
    {
        EL("""", 0);
    }
";

            AreEqualCode(expected, newTree);
        }

        private static AddExecuteLoggerVisitor CreateAddExecuteLoggerVisitor()
        {
            return new AddExecuteLoggerVisitor
            {
                LogAssignMember = "LA",
                BeginLoopMember = "BL",
                EndLoopMember = "EL"
            };
        }

        private static void AreEqualCode(string expected, SyntaxNode tree)
        {
            Assert.AreEqual(RemoveEmptyChars(expected), RemoveEmptyChars(tree.ToString()));
        }

        private static string RemoveEmptyChars(string value)
        {
            return value.Replace("\r\n", "").Replace(" ", "").Replace("\t", "");
        }

        private static StatementSyntax ParseMethodBody(string source)
        {
            var fullSource = "class C { void M() { " + source + "; } }";
            var syntaxTree = CSharpSyntaxTree.ParseText(fullSource);
            var compilationUnit = (CompilationUnitSyntax)syntaxTree.GetRoot();
            var typeDeclaration = (TypeDeclarationSyntax)compilationUnit.Members[0];
            var methodDeclaration = (MethodDeclarationSyntax)typeDeclaration.Members[0];
            var methodBody = methodDeclaration.Body;
            var firstStatement = methodBody.Statements[0];
            return firstStatement;
        }
    }
}

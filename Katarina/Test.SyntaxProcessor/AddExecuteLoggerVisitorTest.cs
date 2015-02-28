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

            var visitor = new AddExecuteLoggerVisitor();
            var newTree = visitor.Visit(tree);

            Assert.AreEqual(@"var a = AV.Cyclone.Katrina.Executor.Context.ExecuteLogger.LogAssign(""a"","""",0,0);", newTree.ToString());
        }

        [Test]
        public void VariableAssignTest()
        {
            var source = "a = 0";
            var tree = ParseMethodBody(source);

            var visitor = new AddExecuteLoggerVisitor();
            var newTree = visitor.Visit(tree);

            Assert.AreEqual(@"a = AV.Cyclone.Katrina.Executor.Context.ExecuteLogger.LogAssign(""a"","""",0,0);", newTree.ToString());
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

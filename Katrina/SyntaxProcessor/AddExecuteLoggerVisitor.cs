using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AV.Cyclone.Katrina.SyntaxProcessor
{
    public class AddExecuteLoggerVisitor : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            var variableName = node.Identifier.Text;
            var fileName = node.Identifier.SyntaxTree.FilePath;
            var lineNumber = node.Identifier.SyntaxTree.GetLineSpan(node.Identifier.Span).StartLinePosition.Line;

            var valueExpression = node.Initializer.Value;
            var memberAccess = CreateMemberAccess("AV.Cyclone.Katrina.Executor.Context.ExecuteLogger.LogAssign");
            var arguments = SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new []
            {
                SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,
                    SyntaxFactory.Literal(variableName))),
                SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,
                    SyntaxFactory.Literal(fileName))),
                SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal(lineNumber))),
                SyntaxFactory.Argument(valueExpression)
            }));
            var invocation = SyntaxFactory.InvocationExpression(memberAccess, arguments);
            return node.Update(node.Identifier, node.ArgumentList, node.Initializer.Update(node.Initializer.EqualsToken, invocation));
        }

        public static ExpressionSyntax CreateMemberAccess(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            var parts = path.Split('.');
            ExpressionSyntax part = SyntaxFactory.IdentifierName(parts[0]);
            for (var i = 1; i < parts.Length; i++)
            {
                part = SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    part,
                    SyntaxFactory.IdentifierName(parts[i]));
            }
            return part;
        }
    }
}
using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AV.Cyclone.Katrina.SyntaxProcessor
{
    public class AddExecuteLoggerVisitor : CSharpSyntaxRewriter
    {
        public string LogAssignMember { get; set; } = "AV.Cyclone.Katrina.Executor.Context.ExecuteLogger.LogAssign";

        public override SyntaxNode VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            var variableName = node.Identifier.Text;
            var fileName = node.SyntaxTree.FilePath;
            var lineNumber = node.SyntaxTree.GetLineSpan(node.Span).StartLinePosition.Line;

            var invocation = CreateLogAssignInvocationExpression(variableName, fileName, lineNumber, node.Initializer.Value);

            return node.Update(node.Identifier, node.ArgumentList, node.Initializer.Update(node.Initializer.EqualsToken, invocation));
        }

        public override SyntaxNode VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            var variableName = node.Left.ToString();
            var fileName = node.SyntaxTree.FilePath;
            var lineNumber = node.SyntaxTree.GetLineSpan(node.Span).StartLinePosition.Line;

            var invocation = CreateLogAssignInvocationExpression(variableName, fileName, lineNumber, node.Right);

            return node.Update(node.Left, node.OperatorToken, invocation);
        }

        private InvocationExpressionSyntax CreateLogAssignInvocationExpression(string variableName, string fileName,
            int lineNumber, ExpressionSyntax valueExpression)
        {
            var memberAccess = CreateMemberAccess(LogAssignMember);
            var arguments = SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[]
            {
                SyntaxFactory.Argument(CreaetLiteral(variableName)),
                SyntaxFactory.Argument(CreaetLiteral(fileName)),
                SyntaxFactory.Argument(CreateLiteral(lineNumber)),
                SyntaxFactory.Argument(valueExpression)
            }));
            return SyntaxFactory.InvocationExpression(memberAccess, arguments);
        }

        private static LiteralExpressionSyntax CreaetLiteral(string value)
        {
            return SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(value));
        }

        private static LiteralExpressionSyntax CreateLiteral(int lineNumber)
        {
            return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                SyntaxFactory.Literal(lineNumber));
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
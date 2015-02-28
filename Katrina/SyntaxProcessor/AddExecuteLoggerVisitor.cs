using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AV.Cyclone.Katrina.SyntaxProcessor
{
    public class AddExecuteLoggerVisitor : CSharpSyntaxRewriter
    {
        public string LogAssignMember { get; set; } = "AV.Cyclone.Katrina.Executor.Context.ExecuteLogger.LogAssign";

        public string BeginLoopMember { get; set; } = "AV.Cyclone.Katrina.Executor.Context.ExecuteLogger.BeginLoop";

        public string LoopIterationMember { get; set; } = "AV.Cyclone.Katrina.Executor.Context.ExecuteLogger.LoopIteration";

        public string EndLoopMember { get; set; } = "AV.Cyclone.Katrina.Executor.Context.ExecuteLogger.EndLoop";

        public override SyntaxNode VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            var variableName = node.Identifier.Text;

            var invocation = CreateLogAssignInvocationExpression(node, variableName, node.Initializer.Value);

            return node.Update(node.Identifier, node.ArgumentList, node.Initializer.Update(node.Initializer.EqualsToken, invocation));
        }

        public override SyntaxNode VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            var variableName = node.Left.ToString();

            var invocation = CreateLogAssignInvocationExpression(node, variableName, node.Right);

            return node.Update(node.Left, node.OperatorToken, invocation);
        }

        public override SyntaxNode VisitWhileStatement(WhileStatementSyntax node)
        {
            var beginLoopInvocation = CreateBeginLoopInvocationExpression(node);
            var endLoopInvocation = CreateEndLoopInvocationExpression(node);

            var whileKeyword = VisitToken(node.WhileKeyword);
            var openParenToken = VisitToken(node.OpenParenToken);
            var condition = (ExpressionSyntax)Visit(node.Condition);
            var closeParenToken = VisitToken(node.CloseParenToken);
            var statement = (StatementSyntax)Visit(node.Statement);

            var loopIterationInvocation = CreateLoopIterationInvocationExpression(statement);
            var whileBody = SyntaxFactory.Block(
                SyntaxFactory.ExpressionStatement(loopIterationInvocation),
                statement
                );

            node = node.Update(whileKeyword, openParenToken, condition, closeParenToken, whileBody);

            var tryBlock = SyntaxFactory.Block(
                SyntaxFactory.ExpressionStatement(beginLoopInvocation),
                node);

            var finallyBlock = SyntaxFactory.FinallyClause(SyntaxFactory.Block(
                SyntaxFactory.ExpressionStatement(endLoopInvocation)));

            return SyntaxFactory.TryStatement(tryBlock, SyntaxFactory.List<CatchClauseSyntax>(), finallyBlock);
        }

        private InvocationExpressionSyntax CreateLogAssignInvocationExpression(SyntaxNode node, string variableName,
            ExpressionSyntax valueExpression)
        {
            var fileName = node.SyntaxTree.FilePath;
            var lineNumber = node.SyntaxTree.GetLineSpan(node.Span).StartLinePosition.Line;

            return CreateLogAssignInvocationExpression(variableName, fileName, lineNumber, valueExpression);
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

        private InvocationExpressionSyntax CreateBeginLoopInvocationExpression(SyntaxNode node)
        {
            var fileName = node.SyntaxTree.FilePath;
            var lineNumber = node.SyntaxTree.GetLineSpan(node.Span).StartLinePosition.Line;

            return CreateBeginLoopInvocationExpression(fileName, lineNumber);
        }

        private InvocationExpressionSyntax CreateBeginLoopInvocationExpression(string fileName, int lineNumber)
        {
            var memberAccess = CreateMemberAccess(BeginLoopMember);
            var arguments = SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[]
            {
                SyntaxFactory.Argument(CreaetLiteral(fileName)),
                SyntaxFactory.Argument(CreateLiteral(lineNumber)),
            }));
            return SyntaxFactory.InvocationExpression(memberAccess, arguments);
        }

        private InvocationExpressionSyntax CreateEndLoopInvocationExpression(SyntaxNode node)
        {
            var fileName = node.SyntaxTree.FilePath;
            var lineNumber = node.SyntaxTree.GetLineSpan(node.Span).StartLinePosition.Line;

            return CreateEndLoopInvocationExpression(fileName, lineNumber);
        }

        private InvocationExpressionSyntax CreateEndLoopInvocationExpression(string fileName, int lineNumber)
        {
            var memberAccess = CreateMemberAccess(EndLoopMember);
            var arguments = SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[]
            {
                SyntaxFactory.Argument(CreaetLiteral(fileName)),
                SyntaxFactory.Argument(CreateLiteral(lineNumber)),
            }));
            return SyntaxFactory.InvocationExpression(memberAccess, arguments);
        }

        private InvocationExpressionSyntax CreateLoopIterationInvocationExpression(SyntaxNode node)
        {
            var fileName = node.SyntaxTree.FilePath;
            var lineNumber = node.SyntaxTree.GetLineSpan(node.Span).StartLinePosition.Line;

            return CreateLoopIterationInvocationExpression(fileName, lineNumber);
        }

        private InvocationExpressionSyntax CreateLoopIterationInvocationExpression(string fileName, int lineNumber)
        {
            var memberAccess = CreateMemberAccess(LoopIterationMember);
            var arguments = SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[]
            {
                SyntaxFactory.Argument(CreaetLiteral(fileName)),
                SyntaxFactory.Argument(CreateLiteral(lineNumber)),
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
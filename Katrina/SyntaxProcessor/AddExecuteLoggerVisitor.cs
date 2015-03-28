using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AV.Cyclone.Katrina.SyntaxProcessor
{
    public class AddExecuteLoggerVisitor : CSharpSyntaxRewriter
    {
        public string LogAssignMember { get; set; }

        public string BeginLoopMember { get; set; }

        public string LoopIterationMember { get; set; }

        public string EndLoopMember { get; set; }

        public string BeginMethodMember { get; set; }

        public string EndMethodMember { get; set; }

        public AddExecuteLoggerVisitor(bool visitIntoStructuredTrivia = false) : base(visitIntoStructuredTrivia)
        {
            LogAssignMember = "AV.Cyclone.Katrina.Executor.Interfaces.Context.ExecuteLoggerHelper.LogAssign";
            BeginLoopMember = "AV.Cyclone.Katrina.Executor.Interfaces.Context.ExecuteLoggerHelper.BeginLoop";
            LoopIterationMember = "AV.Cyclone.Katrina.Executor.Interfaces.Context.ExecuteLoggerHelper.LoopIteration";
            EndLoopMember = "AV.Cyclone.Katrina.Executor.Interfaces.Context.ExecuteLoggerHelper.EndLoop";
            BeginMethodMember = "AV.Cyclone.Katrina.Executor.Interfaces.Context.ExecuteLoggerHelper.BeginMethod";
            EndMethodMember = "AV.Cyclone.Katrina.Executor.Interfaces.Context.ExecuteLoggerHelper.EndMethod";
        }

        public override SyntaxNode VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            if (node.Initializer == null)
                return node;
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

        public override SyntaxNode VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
        {
            var variableName = node.Operand.ToString();

            var invocation = CreateLogAssignInvocationExpression(node, variableName, node);

            return invocation;
        }

        public override SyntaxNode VisitIfStatement(IfStatementSyntax node)
        {
            var ifKeyword = VisitToken(node.IfKeyword);
            var openParenToken = VisitToken(node.OpenParenToken);
            var condition = (ExpressionSyntax)Visit(node.Condition);
            condition = CreateLogAssignInvocationExpression(node, "if", condition);
            var closeParenToken = VisitToken(node.CloseParenToken);
            var statement = (StatementSyntax)Visit(node.Statement);
            var @else = (ElseClauseSyntax)Visit(node.Else);
            return node.Update(ifKeyword, openParenToken, condition, closeParenToken, statement, @else);
        }

        public override SyntaxNode VisitWhileStatement(WhileStatementSyntax node)
        {
            var beginLoopInvocation = CreateBeginLoopInvocationExpression(node);
            var endLoopInvocation = CreateEndLoopInvocationExpression(node);

            var whileKeyword = VisitToken(node.WhileKeyword);
            var openParenToken = VisitToken(node.OpenParenToken);
            var condition = (ExpressionSyntax)Visit(node.Condition);
            condition = CreateLogAssignInvocationExpression(node, "while", condition);
            var closeParenToken = VisitToken(node.CloseParenToken);
            var statement = (StatementSyntax)Visit(node.Statement);

            var loopIterationInvocation = CreateLoopIterationInvocationExpression(statement);
            var whileBody = SyntaxFactory.Block(
                statement,
                SyntaxFactory.ExpressionStatement(loopIterationInvocation));

            node = node.Update(whileKeyword, openParenToken, condition, closeParenToken, whileBody);

            var tryBlock = SyntaxFactory.Block(
                SyntaxFactory.ExpressionStatement(beginLoopInvocation),
                node);

            var finallyBlock = SyntaxFactory.FinallyClause(SyntaxFactory.Block(
                SyntaxFactory.ExpressionStatement(endLoopInvocation)));

            return SyntaxFactory.TryStatement(tryBlock, SyntaxFactory.List<CatchClauseSyntax>(), finallyBlock);
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var methodName = node.Identifier.Text;

            var attributeLists = VisitList(node.AttributeLists);
            var modifiers = VisitList(node.Modifiers);
            var returnType = (TypeSyntax)Visit(node.ReturnType);
            var explicitInterfaceSpecifier = (ExplicitInterfaceSpecifierSyntax)Visit(node.ExplicitInterfaceSpecifier);
            var identifier = VisitToken(node.Identifier);
            var typeParameterList = (TypeParameterListSyntax)Visit(node.TypeParameterList);
            var parameterList = (ParameterListSyntax)Visit(node.ParameterList);
            var constraintClauses = VisitList(node.ConstraintClauses);
            var body = (BlockSyntax)Visit(node.Body);

            var beginLoopInvocation = CreateBeginMethodInvocationExpression(methodName, node);
            var endLoopInvocation = CreateEndMethodInvocationExpression(methodName, node);

            var expressionBody = (ArrowExpressionClauseSyntax)Visit(node.ExpressionBody);

            var tryBlock = SyntaxFactory.Block(
                SyntaxFactory.ExpressionStatement(beginLoopInvocation),
                body);

            var finallyBlock = SyntaxFactory.FinallyClause(SyntaxFactory.Block(
                SyntaxFactory.ExpressionStatement(endLoopInvocation)));

            var tryFinallyBlock = SyntaxFactory.Block(
                SyntaxFactory.TryStatement(tryBlock, SyntaxFactory.List<CatchClauseSyntax>(),
                    finallyBlock));

            var semicolonToken = VisitToken(node.SemicolonToken);
            return node.Update(attributeLists, modifiers, returnType, explicitInterfaceSpecifier, identifier,
                typeParameterList, parameterList, constraintClauses, tryFinallyBlock, expressionBody, semicolonToken);
        }

        public override SyntaxNode VisitReturnStatement(ReturnStatementSyntax node)
        {
            var invocation = CreateLogAssignInvocationExpression(node, "return",
                (ExpressionSyntax)base.Visit(node.Expression));
            return node.Update(node.ReturnKeyword, invocation, node.SemicolonToken);
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

        private InvocationExpressionSyntax CreateBeginMethodInvocationExpression(string methodName, SyntaxNode node)
        {
            var fileName = node.SyntaxTree.FilePath;
            var lineNumber = node.SyntaxTree.GetLineSpan(node.Span).StartLinePosition.Line;

            return CreateBeginMethodInvocationExpression(methodName, fileName, lineNumber);
        }

        private InvocationExpressionSyntax CreateBeginMethodInvocationExpression(string methodName, string fileName, int lineNumber)
        {
            var memberAccess = CreateMemberAccess(BeginMethodMember);
            var arguments = SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[]
            {
                SyntaxFactory.Argument(CreaetLiteral(methodName)),
                SyntaxFactory.Argument(CreaetLiteral(fileName)),
                SyntaxFactory.Argument(CreateLiteral(lineNumber)),
            }));
            return SyntaxFactory.InvocationExpression(memberAccess, arguments);
        }

        private InvocationExpressionSyntax CreateEndMethodInvocationExpression(string methodName, SyntaxNode node)
        {
            var fileName = node.SyntaxTree.FilePath;
            var lineNumber = node.SyntaxTree.GetLineSpan(node.Span).StartLinePosition.Line;

            return CreateEndMethodInvocationExpression(methodName, fileName, lineNumber);
        }

        private InvocationExpressionSyntax CreateEndMethodInvocationExpression(string methodName, string fileName, int lineNumber)
        {
            var memberAccess = CreateMemberAccess(EndMethodMember);
            var arguments = SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[]
            {
                SyntaxFactory.Argument(CreaetLiteral(methodName)),
                SyntaxFactory.Argument(CreaetLiteral(fileName)),
                SyntaxFactory.Argument(CreateLiteral(lineNumber)),
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
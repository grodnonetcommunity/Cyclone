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
            var identifier = node.Identifier;
            return base.VisitVariableDeclarator(node);
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
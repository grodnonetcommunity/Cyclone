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
    }
}
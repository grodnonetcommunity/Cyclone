using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AV.Cyclone.Katrina.Executor
{
    public class ForecastItem
    {
        public CSharpCompilation Compilation { get; set; }

        public SyntaxTree SyntaxTree { get; set; }
    }
}
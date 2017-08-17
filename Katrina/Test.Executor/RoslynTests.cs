using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;

namespace Test.Executor
{
    public class RoslynTests
    {
        private const string SolutionFolderPath = @"..\..\.TestSolution\";
        private static readonly string binarySearchFilePath = GetFullFileName(Path.Combine(SolutionFolderPath, @"Algorithms\BinarySearch.cs"));
        private static readonly string testFilePath = GetFullFileName(Path.Combine(SolutionFolderPath, @"Test.Algorithms\BinarySerchTest.cs"));
        private static readonly string nunitAssmeblyPath = Path.Combine(SolutionFolderPath, @"packages\NUnit.2.6.4\lib\nunit.framework.dll");

        [Test]
        public void CompilationReferences()
        {
            SyntaxTree binarySearchFileTree;
            SyntaxTree testFileTree;
            using (var binarySearchFileStream = new FileStream(binarySearchFilePath, FileMode.Open))
            using (var testFilePathStream = new FileStream(testFilePath, FileMode.Open))
            {
                binarySearchFileTree = CSharpSyntaxTree.ParseText(SourceText.From(binarySearchFileStream, Encoding.UTF8))
                    .WithFilePath(binarySearchFilePath);
                testFileTree = CSharpSyntaxTree.ParseText(SourceText.From(testFilePathStream, Encoding.UTF8))
                    .WithFilePath(testFilePath);
            }

            var mscorlibReference = MetadataReference.CreateFromFile(typeof(Object).Assembly.Location);
            var nuinitReference = MetadataReference.CreateFromFile(GetFullFileName(nunitAssmeblyPath));

            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var algorithmsCompilation = CSharpCompilation.Create("Algorithms.dll", new[] { binarySearchFileTree },
                new[] { mscorlibReference }, compilationOptions);
            var testCompilation = CSharpCompilation.Create("Test.Algorithms.dll", new[] { testFileTree },
                new[] { mscorlibReference, nuinitReference }, compilationOptions);

            var algorithmsPeStream = new MemoryStream();
            var algorithmsPdbStream = new MemoryStream();

            var algorithmsEmitResult = algorithmsCompilation.Emit(algorithmsPeStream, algorithmsPdbStream);
            Assert.IsTrue(algorithmsEmitResult.Success);

            var algorithmsPe = algorithmsPeStream.ToArray();

            testCompilation = testCompilation.AddReferences(MetadataReference.CreateFromImage(algorithmsPe));

            var testAlgorithmsPeStream = new MemoryStream();
            var testAlgorithmsPdbStream = new MemoryStream();

            var testAlgorithmsEmitResult = testCompilation.Emit(testAlgorithmsPeStream, testAlgorithmsPdbStream);
            Assert.IsTrue(testAlgorithmsEmitResult.Success);
        }

        private static string GetFullFileName(string solutionPath, [CallerFilePath] string fileName = null)
        {
            var result = new StringBuilder();
            var combinedPath = Path.Combine(Path.GetDirectoryName(fileName), solutionPath);
            return Path.GetFullPath(combinedPath);
        }
    }
}
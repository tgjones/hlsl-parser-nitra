using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nitra;
using NUnit.Framework;
using SharpDX.D3DCompiler;

namespace HlslParser.Tests
{
    [TestFixture]
    public class HlslParserTests
    {
        [TestCaseSource("GetTestShaders")]
        public void CanParseShader(string testFile, string knownGoodFile)
        {
            // Preprocess test code using D3DCompiler.
            var sourceCode = ShaderBytecode.Preprocess(File.ReadAllText(testFile), 
                include: new FileSystemInclude(Path.GetDirectoryName(testFile)));

            // Parse test code.
            var sourceSnapshot = new SourceSnapshot(sourceCode);
            var parserHost = new ParserHost();
            var compilationUnit = HlslGrammar.CompilationUnit(sourceSnapshot, parserHost);

            // Check for parse errors.
            if (!compilationUnit.IsSuccess)
            {
                throw new Exception(string.Join(Environment.NewLine,
                    compilationUnit.GetErrors().Select(x => string.Format("Line {0}, Col {1}: {2}{3}{4}",
                        x.Location.StartLineColumn.Line, x.Location.StartLineColumn.Column, x.Message,
                        Environment.NewLine, x.Location.Source.GetSourceLine(x.Location.StartPos).GetText()))));
            }

            Assert.That(compilationUnit.IsSuccess, Is.True);

            // Get pretty-printed version of parse tree.
            var parseTree = compilationUnit.CreateParseTree();
            var parsedCode = parseTree.ToString();

            // Compare pretty-printed parse tree with known good version
            // (if known good version exists).
            if (File.Exists(knownGoodFile))
            {
                var knownGoodCode = File.ReadAllText(knownGoodFile);
                Assert.That(parsedCode, Is.EqualTo(knownGoodCode));
            }
        }

        private static IEnumerable<TestCaseData> GetTestShaders()
        {
            return Directory.GetFiles("Shaders", "*.hlsl", SearchOption.AllDirectories)
                .Select(x => new TestCaseData(x, Path.ChangeExtension(x, ".knowngood")));
        }
    }
}

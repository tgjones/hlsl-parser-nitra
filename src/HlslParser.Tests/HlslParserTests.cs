using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CppNet;
using Nitra;
using NUnit.Framework;

namespace HlslParser.Tests
{
    [TestFixture]
    public class HlslParserTests
    {
        [TestCaseSource("GetTestShaders")]
        public void CanParseShader(string testFile, string knownGoodFile)
        {
            // Preprocess test code using CppNet.
            var sourceCode = Preprocess(File.ReadAllText(testFile), testFile);

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
                var knownGoodCode = File.ReadAllText(knownGoodFile).Replace("\r\n", "\n");
                Assert.That(parsedCode.Replace("\r\n", "\n"), Is.EqualTo(knownGoodCode));
            }
        }

        private static IEnumerable<TestCaseData> GetTestShaders()
        {
            return Directory.GetFiles("Shaders", "*.hlsl", SearchOption.AllDirectories)
                .Select(x => new TestCaseData(x, Path.ChangeExtension(x, ".knowngood")));
        }

        private static string Preprocess(string effectCode, string filePath)
        {
            var fullPath = Path.GetFullPath(filePath);

            var pp = new Preprocessor();

            pp.EmitExtraLineInfo = false;
            pp.addFeature(Feature.LINEMARKERS);
            pp.setQuoteIncludePath(new List<string> { Path.GetDirectoryName(fullPath) });

            pp.addInput(new StringLexerSource(effectCode, true, fullPath));

            var result = new StringBuilder();

            var endOfStream = false;
            while (!endOfStream)
            {
                var token = pp.token();
                switch (token.getType())
                {
                    case Token.EOF:
                        endOfStream = true;
                        break;
                    case Token.CCOMMENT:
                    case Token.CPPCOMMENT:
                        break;
                    default:
                        var tokenText = token.getText();
                        if (tokenText != null)
                            result.Append(tokenText);
                        break;
                }
            }

            return result.ToString();
        }
    }
}

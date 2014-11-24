HlslParser
==========

HlslParser is a SM5.0 HLSL parser for .NET. The grammar and parser are built using JetBrains'
[Nitra](https://github.com/JetBrains/Nitra) project.
Currently, it only parses HLSL code into an AST. Once Nitra adds support for it
(planned for a future milestone), I plan to extend HlslParser to support full 
HLSL intellisense in Visual Studio.

HlslParser is currently capable of parsing most / all of the sample shaders in the Direct3D SDK,
as well as several other shaders - see the [test suite](src/HlslParser.Tests/Shaders).

(On a nerd note, Nitra makes it possible to define the grammar in a nice concise way; so far, I prefer it to
other parser generators that I've used. [Here is the HLSL grammar file](src/HlslParser/HlslGrammar.nitra).)

Usage
-----

```csharp
var sourceSnapshot = new SourceSnapshot(sourceCode);
var parserHost = new ParserHost();
var compilationUnit = HlslGrammar.CompilationUnit(sourceSnapshot, parserHost);

Assert.That(compilationUnit.IsSuccess, Is.True);

var parseTree = compilationUnit.CreateParseTree();
var parsedCode = parseTree.ToString();
```

Prerequisites
-------------

Because HlslParser is based on Nitra, you'll need to install the following before you can
build HlslParser on your own machine:

1. [Nemerle](http://nemerle.org/Downloads)
2. [Nitra](http://nemerle.org/nitra-builds/)

I haven't built an HLSL preprocessor. Instead, I make use of the Direct3D HLSL preprocessor, through SharpDX.
SharpDX requires a specific DirectX runtime, so if you want to run the
HlslParser test suite, you'll need to install that too:

* [DirectX June 2010 Redistributable Runtime](http://www.microsoft.com/en-us/download/details.aspx?id=8109)

License
-------

HlslParser is released under the [MIT License](http://www.opensource.org/licenses/MIT).
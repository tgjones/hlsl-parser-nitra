using System;
using System.IO;
using SharpDX.D3DCompiler;

namespace HlslParser.Tests
{
    internal class FileSystemInclude : Include
    {
        private readonly string _folder;

        public IDisposable Shadow { get; set; }

        public FileSystemInclude(string folder)
        {
            _folder = folder;
        }

        public Stream Open(IncludeType type, string fileName, Stream parentStream)
        {
            return File.OpenRead(Path.Combine(_folder, fileName));
        }

        public void Close(Stream stream)
        {
            stream.Dispose();
        }

        public void Dispose()
        {

        }
    }
}
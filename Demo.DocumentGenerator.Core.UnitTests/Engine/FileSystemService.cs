using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Demo.DocumentGenerator.Core.UnitTests
{
    public class FileSystemService : IFileSystemService
    {
        public string ReadAllText(string fileName)
        {
            return File.ReadAllText(fileName);
        }

        public bool FileExists(string fileName)
        {
            return File.Exists(fileName);
        }

        public string GetCurrentDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }
}

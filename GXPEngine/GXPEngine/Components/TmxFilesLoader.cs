using System;
using System.IO;
using System.Linq;

namespace GXPEngine
{
    /// <summary>
    /// Load TMX file names in Debug/Release Folder
    /// </summary>
    public class TmxFilesLoader
    {
        public static string[] GetTmxFileNames(string pattern = "*.tmx")
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var files = new DirectoryInfo(baseDir)?.GetFiles(pattern);
            
            return files?.Select(f => f.FullName).ToArray();
        }
    }
}
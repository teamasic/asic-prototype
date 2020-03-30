using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Utils
{
    public class FileUtils
    {
        public static string GetTempFilePathWithExtension(string extension)
        {
            var path = Path.GetTempPath();
            var fileName = Guid.NewGuid().ToString() + extension;
            return Path.Combine(path, fileName);
        }

        public static string GetFolderRelativeToStartupFile(string folder)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), folder);
        }
        public static string GetFolderRelativeToBaseDirectory(string folder)
        {
            var currentDirectory = Environment.CurrentDirectory;
            var parentDirectory = Directory.GetParent(currentDirectory).FullName;
            return Path.Combine(parentDirectory, folder);
        }
    }
}

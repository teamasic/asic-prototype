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
    }
}

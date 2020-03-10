using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.AppSettingConfiguration
{
    public class MyConfiguration
    {
        public string PythonExeFullPath { get; set; }
        public string RecognitionProgramPathOpenCV { get; set; }        
        public string RecognitionProgramPathVLC { get; set; }
        public string RecognitionServiceName { get; set; }
        public string ExportFilePath { get; set; }
        public string RecognitionImageBase64Path { get; set; }
    }
}

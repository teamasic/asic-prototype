using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.AppSettingConfiguration
{
    public class MyConfiguration
    {
        public string PythonExeFullPath { get; set; }
        public string RecognitionProgramPathVLC { get; set; }
        public string RecognitionServiceName { get; set; }
        public string ExportFilePath { get; set; }
        public string RecognitionImageBase64Path { get; set; }
        public string ServerUrl { get; set; }
        public string AvatarFolderPath { get; set; }
        public string UnknownFolderPath { get; set; }
        public string RecognizedFolderPath { get; set; }
        public string AvatarPlaceholderName { get; set; }
        public string RecognitionProgramMultiplePath { get; set; }
        public string LogoFolderPath { get; set; }
    }
}

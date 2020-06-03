using AttendanceSystemIPCamera.Framework.AppSettingConfiguration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Utils
{
    public class DownloadUtils
    {
        public static string DownloadImageFromUrl(string imageUrl, string code, MyConfiguration myConfiguration)
        {
            var destinationExt = Path.GetExtension(imageUrl);
            var destinationFileName = code + destinationExt;
            var destinationDir = myConfiguration.AvatarFolderPath + "\\";
            var destinationPath = destinationDir + destinationFileName;
            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            //Download file from remote url and save it in destination dir
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(imageUrl, destinationPath);
            }
            return destinationFileName;
        }
    }
}

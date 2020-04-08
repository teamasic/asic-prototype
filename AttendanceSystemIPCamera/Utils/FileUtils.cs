using AttendanceSystemIPCamera.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static AttendanceSystemIPCamera.Framework.Constants;

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
        public static DateTime GetLatestSyncTime()
        {
            string content = File.ReadAllText(Constant.LATEST_SYNC_PATH);
            DateTime latestSyncTime;
            if(DateTime.TryParse(content, out latestSyncTime))
            {
                return latestSyncTime;
            }
            throw new BaseException(ErrorMessage.LATEST_SYNC_NOT_VALID);
        }

        public static void UpdateLatestSyncTime(DateTime dateTime)
        {
            File.WriteAllText(Constant.LATEST_SYNC_PATH, dateTime.ToString());
        }
    
        public static DateTime GetShutDownTime()
        {
            string content = File.ReadAllText(Constant.SHUT_DOWN_PATH);
            DateTime shutdownTime;
            if (DateTime.TryParse(content, out shutdownTime))
            {
                return shutdownTime;
            }
            return DateTime.Now.AddMilliseconds(Constant.DEFAULT_SYNC_MILISECONDS);
        }

        public static void UpdateShutDownTime(DateTime dateTime)
        {
            File.WriteAllText(Constant.SHUT_DOWN_PATH, dateTime.ToString());
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

        public static string ReadFile(string fileName)
        {
            return File.ReadAllText(fileName);
        }

        public static T ParseJsonFile<T>(string fileName) where T : new()
        {
            T expectedOutput = new T();
            try
            {
                using StreamReader file = File.OpenText(fileName);
                JsonSerializer serializer = new JsonSerializer();
                expectedOutput = (T)serializer.Deserialize(file, typeof(T));
            }
            catch (Exception)
            {
            }
            return expectedOutput;
        }
        public static void UpdateJsonFile(string fileName, object newObject)
        {
            try
            {
                var contents = JsonConvert.SerializeObject(newObject);
                File.WriteAllText(fileName, contents);
            }
            catch (Exception)
            {
            }
        }
    }
}

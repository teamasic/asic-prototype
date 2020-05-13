using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;

namespace SupervisorApp.Utils
{
    public class Utils
    {
        public const string CONFIG_FILE = "wpfsettings.json";
        private static Config config = null;

        public static Config GetConfig()
        {
            if (config == null)
            {
                if (File.Exists(CONFIG_FILE))
                {
                    string content = File.ReadAllText(CONFIG_FILE);
                    config = JsonConvert.DeserializeObject<Config>(content);
                }
                else
                {
                    config = new Config();
                }
            }
            return config;
        }
    }
}

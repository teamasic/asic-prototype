using AttendanceSystemIPCamera.Framework.AppSettingConfiguration;
using AttendanceSystemIPCamera.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Services.UnitService
{
    class UnitUtils
    {
        public static ICollection<Unit> GetUnits(string unitConfigFile)
        {
            ICollection<Unit> units = new List<Unit>();
            try
            {
                using StreamReader file = File.OpenText(unitConfigFile);
                JsonSerializer serializer = new JsonSerializer();
                units = (ICollection<Unit>)serializer.Deserialize(file, typeof(ICollection<Unit>));
            }
            catch (Exception)
            {
            }
            return units;
        }
    }
    public class UnitServiceFactory
    {
        public static UnitService UnitService { get; private set; }
        public static UnitService Create(string unitConfigFile)
        {
            if (UnitService != null)
            {
                return UnitService;
            }
            UnitService = new UnitService
            {
                ConfigFile = unitConfigFile,
                Units = UnitUtils.GetUnits(unitConfigFile)
            };
            return UnitService;
        }
    }
    public class UnitService
    {
        public string ConfigFile { get; set; }
        public ICollection<Unit> Units { get; set; }

        public void Refresh()
        {
            this.Units = UnitUtils.GetUnits(ConfigFile);
        }

        public ICollection<Unit> GetUnitsForToday()
        {
            return Units.Select(u => new Unit
            {
                Id = u.Id,
                Name = u.Name,
                StartTime = DateTime.Today + u.StartTime.TimeOfDay,
                EndTime = DateTime.Today + u.EndTime.TimeOfDay
            }).OrderBy(u => u.StartTime).ToList();
        }
    }
}

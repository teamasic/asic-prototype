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
    public class UnitServiceFactory
    {
        public static UnitService UnitService { get; private set; }
        public static UnitService Create(String unitConfigFile)
        {
            if (UnitService != null)
            {
                return UnitService;
            }
            ICollection<Unit> units = new List<Unit>();
            try
            {
                using (StreamReader file = File.OpenText(unitConfigFile))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    units = (ICollection<Unit>)serializer.Deserialize(file, typeof(ICollection<Unit>));
                }
            }
            catch (Exception)
            {
            }
            UnitService = new UnitService
            {
                Units = units
            };
            return UnitService;
        }
    }
    public class UnitService
    {
        public ICollection<Unit> Units { get; set; }

        public ICollection<Unit> GetUnitsForToday()
        {
            return Units.Select(u => new Unit
            {
                Name = u.Name,
                StartTime = DateTime.Today + u.StartTime.TimeOfDay,
                EndTime = DateTime.Today + u.EndTime.TimeOfDay
            }).OrderBy(u => u.StartTime).ToList();
        }
    }
}

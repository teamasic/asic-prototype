using AttendanceSystemIPCamera.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class SessionViewModel : BaseViewModel<Session>
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public bool Active { get; set; }
        public int GroupId { get; set; }
        public RecordViewModel Record { get; set; }
    }

    public class SessionNetworkViewModel : BaseViewModel<Session>
    {
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        [JsonIgnore]
        public int GroupId { get; set; }
        public List<RecordNetworkViewModel> Records { get; set; }
    }

}

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
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int GroupId { get; set; }
        public RecordViewModel Record { get; set; }
        [JsonIgnore]
        public GroupViewModel Group { get; set; }
    }

    public class SessionNetworkViewModel : BaseViewModel<Session>
    {
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string RtspString { get; set; }
        public string RoomName { get; set; }
        [JsonIgnore]
        public int GroupId { get; set; }
        public List<RecordNetworkViewModel> Records { get; set; }
    }

    public class SessionInSyncData : BaseViewModel<Session>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string RtspString { get; set; }
        public string RoomName { get; set; }
        public int GroupId { get; set; }
        public GroupInSyncData Group { get; set; }
    }

    public class SessionNotificationViewModel : BaseViewModel<Session>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string RoomName { get; set; }
    }
}

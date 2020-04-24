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
        public string GroupCode { get; set; }
        public RecordViewModel Record { get; set; }
        [JsonIgnore]
        public GroupViewModel Group { get; set; }
        public RoomViewModel Room { get; set; }
        public string Status { get; set; }
    }

    public class SessionNetworkViewModel : BaseViewModel<Session>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string GroupCode { get; set; }
        public string Status { get; set; }
        public List<RecordNetworkViewModel> Records { get; set; }
    }

    public class SessionInSyncData : BaseViewModel<Session>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; }
        public int RoomId { get; set; }
        public string GroupCode { get; set; }
    }

    public class SessionNotificationViewModel : BaseViewModel<Session>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public string RoomName { get; set; }
    }

    public class SessionRefactorViewModel : BaseViewModel<Session>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public RoomViewModel room { get; set; }
    }

    public class SessionCreateViewModel
    {
        public string GroupCode { get; set; }
        public string Slot { get; set; }
        public string Room { get; set; }
        public DateTime Date { get; set; }
    }

    public class SessionUpdateRoomViewModal
    {
        public int sessionId { get; set; }
        public int roomId { get; set; }
    }
}

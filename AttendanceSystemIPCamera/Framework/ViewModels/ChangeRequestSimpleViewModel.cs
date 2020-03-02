using AttendanceSystemIPCamera.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class ChangeRequestSimpleViewModel
    {
        public int Id { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public RecordViewModel Record { get; set; }
        public int RecordId => Record.Id;
        public string AttendeeCode => Record.Attendee?.Code;
        public string AttendeeName => Record.Attendee?.Name;
        public int GroupId => Record.Session != null ? Record.Session.Group.Id : 0;
        public string GroupName => Record.Session?.Group.Name;
        public string GroupCode => Record.Session?.Group.Code;
        public DateTime SessionTime => Record.Session != null ? Record.Session.StartTime : DateTime.MinValue;
        public string SessionName => Record.Session?.Name;
        public int SessionId => Record.Session != null ? Record.Session.Id : 0;
        public string Comment { get; set; }
        public bool OldState { get; set; }
        public bool NewState { get; set; }
        public ChangeRequestStatus Status { get; set; } = ChangeRequestStatus.UNRESOLVED;
    }
}

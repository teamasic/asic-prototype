using AttendanceSystemIPCamera.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class ChangeRequestSimpleViewModel
    {
        public int Id { get; set; }
        [JsonIgnore]
        public RecordViewModel Record { get; set; }
        public int RecordId => Record.Id;
        public string AttendeeCode => Record.Attendee.Code;
        public string AttendeeName => Record.Attendee.Name;
        public string GroupName => Record.Session.Group.Name;
        public string GroupCode => Record.Session.Group.Code;
        public DateTime SessionTime => Record.Session.StartTime;
        public string SessionName => Record.Session.Name;
        public string Comment { get; set; }
        public bool OldState { get; set; }
        public bool NewState { get; set; }
        public ChangeRequestStatus Status { get; set; } = ChangeRequestStatus.UNRESOLVED;
    }
}

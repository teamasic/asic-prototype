using System;
using System.Collections.Generic;

namespace AttendanceSystemIPCamera.Models
{
    public partial class Record : BaseEntity
    {
        public Record()
        {
            ChangeRequest = new HashSet<ChangeRequest>();
        }

        public int Id { get; set; }
        public int AttendeeId { get; set; }
        public int GroupId { get; set; }
        public int SessionId { get; set; }
        public bool Present { get; set; }
        public string AttendeeCode { get; set; }
        public string SessionName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime? UpdateTime { get; set; }

        public virtual AttendeeGroup AttendeeGroup { get; set; }
        public virtual Session Session { get; set; }
        public virtual ICollection<ChangeRequest> ChangeRequest { get; set; }
    }
}

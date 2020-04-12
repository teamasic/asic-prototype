using System;
using System.Collections.Generic;

namespace AttendanceSystemIPCamera.Models
{
    public partial class AttendeeGroup
    {
        public AttendeeGroup()
        {
            Record = new HashSet<Record>();
        }

        public int AttendeeId { get; set; }
        public int GroupId { get; set; }
        public bool IsActive { get; set; }

        public virtual Attendee Attendee { get; set; }
        public virtual Group Group { get; set; }
        public virtual ICollection<Record> Record { get; set; }
    }
}

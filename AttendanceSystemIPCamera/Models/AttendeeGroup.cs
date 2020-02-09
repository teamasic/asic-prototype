using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Models
{
    public class AttendeeGroup
    {
        public int AttendeeId { get; set; }
        public virtual Attendee Attendee { get; set; }
        public int GroupId { get; set; }
        public virtual Group Group { get; set; }
    }
}

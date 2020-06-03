using AttendanceSystemIPCamera.Models;
using System;
using System.Collections.Generic;

namespace AttendanceSystemIPCamera.Models
{
    public partial class Session: BaseEntity
    {
        public Session()
        {
            Records = new HashSet<Record>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; }
        public string GroupCode { get; set; }
        public int RoomId { get; set; }

        public virtual Group Group { get; set; }
        public virtual Room Room { get; set; }
        public virtual ICollection<Record> Records { get; set; }
    }
}

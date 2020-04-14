using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace AttendanceSystemIPCamera.Models
{
    public partial class Attendee
    {
        public Attendee()
        {
            AttendeeGroups = new HashSet<AttendeeGroup>();
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }

        public virtual ICollection<AttendeeGroup> AttendeeGroups { get; set; }
        [NotMapped]
        public virtual ICollection<Group> Groups => AttendeeGroups.Select(ag => ag.Group).ToList();
        public override bool Equals(object obj)
        {
            return obj is Attendee attendee &&
                   Code == attendee.Code;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Code);
        }
    }
}

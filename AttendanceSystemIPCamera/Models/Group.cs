using AttendanceSystemIPCamera.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace AttendanceSystemIPCamera.Models
{
    public partial class Group: IDeletable, IHasDateTimeCreated
    {
        public Group()
        {
            AttendeeGroups = new HashSet<AttendeeGroup>();
            Sessions = new HashSet<Session>();
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime DateTimeCreated { get; set; } = DateTime.Now;
        public int TotalSession { get; set; }
        public bool Deleted { get; set; }

        public virtual ICollection<AttendeeGroup> AttendeeGroups { get; set; }
        [NotMapped]
        public ICollection<Attendee> Attendees => AttendeeGroups
            .Where(ag => ag.IsActive)
            .Select(ag => ag.Attendee)
            .ToList();
        public virtual ICollection<Session> Sessions { get; set; }
    }
}

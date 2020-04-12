using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Models
{
    public class Group : BaseEntity, IDeletable, IHasDateTimeCreated
    {
        public Group()
        {
            AttendeeGroups = new HashSet<AttendeeGroup>();
            Sessions = new HashSet<Session>();
            Schedules = new HashSet<Schedule>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime DateTimeCreated { get; set; } = DateTime.Now;
        public int MaxSessionCount { get; set; }
        public bool Deleted { get; set; }
        public virtual ICollection<AttendeeGroup> AttendeeGroups { get; set; }
        [NotMapped]
        public ICollection<Attendee> Attendees => AttendeeGroups.Select(ag => ag.Attendee).ToList();
        public virtual ICollection<Session> Sessions { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Models
{
    public class Group: BaseEntity, IDeletable, IHasDateTimeCreated
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public String Name { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public virtual ICollection<AttendeeGroup> AttendeeGroups { get; set; } = new List<AttendeeGroup>();
        [NotMapped]
        public ICollection<Attendee> Attendees => AttendeeGroups.Select(ag => ag.Attendee).ToList();
        public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
        public bool Deleted { get; set; }
    }
}

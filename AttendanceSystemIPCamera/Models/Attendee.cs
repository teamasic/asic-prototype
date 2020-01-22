using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Models
{
    public class Attendee: BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<AttendeeGroup> AttendeeGroups { get; set; } = new List<AttendeeGroup>();
        [NotMapped]
        public ICollection<Group> Groups => AttendeeGroups.Select(ag => ag.Group).ToList();
        public virtual ICollection<Record> Records { get; set; } = new List<Record>();
    }
}

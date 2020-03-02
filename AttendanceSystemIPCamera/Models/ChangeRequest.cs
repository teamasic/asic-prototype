using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Models
{
    public enum ChangeRequestStatus
    {
        UNRESOLVED = 0,
        APPROVED = 1,
        REJECTED = 2
    }
    public class ChangeRequest: BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public Record Record { get; set; }
        public string Comment { get; set; }
        public bool OldState { get; set; }
        public bool NewState { get; set; }
        public ChangeRequestStatus Status { get; set; } = ChangeRequestStatus.UNRESOLVED;
        [NotMapped]
        public bool IsResolved => Status != ChangeRequestStatus.UNRESOLVED;
    }
}

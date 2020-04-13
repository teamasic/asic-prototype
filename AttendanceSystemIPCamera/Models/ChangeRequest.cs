using AttendanceSystemIPCamera.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceSystemIPCamera.Models
{
    public enum ChangeRequestStatus
    {
        UNRESOLVED = 0,
        APPROVED = 1,
        REJECTED = 2
    }
    public partial class ChangeRequest : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public int RecordId { get; set; }
        public string Comment { get; set; }
        [NotMapped]
        public bool IsResolved => Status != ChangeRequestStatus.UNRESOLVED;

        public virtual Record Record { get; set; }
        public ChangeRequestStatus Status { get; set; } = ChangeRequestStatus.UNRESOLVED;
    }
}

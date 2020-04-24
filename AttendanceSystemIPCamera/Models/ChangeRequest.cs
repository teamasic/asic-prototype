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
        REJECTED = 2,
        EXPIRED = 3
    }
    public partial class ChangeRequest
    {
        [Key]
        public int RecordId { get; set; }
        public string Comment { get; set; }
        public DateTime DateSubmitted { get; set; }
        [NotMapped]
        public bool IsResolved => Status != ChangeRequestStatus.UNRESOLVED;
        public virtual Record Record { get; set; }
        public ChangeRequestStatus Status { get; set; } = ChangeRequestStatus.UNRESOLVED;
    }
}

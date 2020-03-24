using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Models
{
    public class Record : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int AttendeeId { get; set; }
        public int SessionId { get; set; }
        //giảm chuẩn để tiện cho sync attendance data
        public string AttendeeCode { get; set; }
        public string SessionName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Attendee Attendee { get; set; }
        public Session Session { get; set; }
        public bool Present { get; set; }
        public ChangeRequest ChangeRequest { get; set; }
    }
}

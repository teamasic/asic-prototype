using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Models
{
    public class Classroom: BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string RtspString { get; set; }
    }
}

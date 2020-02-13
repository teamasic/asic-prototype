using AttendanceSystemIPCamera.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class RecordViewModel : BaseViewModel<Record>
    {
        public bool Present { get; set; }
    }

    public class RecordAttendanceViewModel
    {
        public int Id { get; set; }
        public string GroupCode { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public bool Present { get; set; }
    }

    public class RecordSearchViewModel
    {
        [Required]
        public int AttendeeId { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
    }

}

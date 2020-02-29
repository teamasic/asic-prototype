using AttendanceSystemIPCamera.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class ChangeRequestViewModel
    {
        public int Id { get; set; }
        public RecordViewModel Record { get; set; }
        public string Comment { get; set; }
        public bool OldState { get; set; }
        public bool NewState { get; set; }
        public ChangeRequestStatus Status { get; set; } = ChangeRequestStatus.UNRESOLVED;
    }
}

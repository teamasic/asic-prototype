using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class ExportRequestViewModel
    {
        public int GroupId { get; set; }
        public bool IsSingleDate { get; set; }
        public bool WithCondition { get; set; }
        public DateTime SingleDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsPresent { get; set; }
        public bool IsGreaterThanOrEqual { get; set; }
        public float AttendancePercent { get; set; }
    }
}

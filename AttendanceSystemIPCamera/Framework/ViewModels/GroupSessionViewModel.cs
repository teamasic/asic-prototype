using AttendanceSystemIPCamera.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class GroupSessionViewModel : BaseViewModel<Group>
    {
        public string GroupCode { get; set; }
        public string Name { get; set; }
        public List<SessionViewModel> Sessions { get; set; }

    }
}

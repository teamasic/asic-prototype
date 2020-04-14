using AttendanceSystemIPCamera.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class GroupViewModel: BaseViewModel<Group>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int TotalSession { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public ICollection<AttendeeViewModel> Attendees { get; set; }
        public ICollection <SessionViewModel> Sessions { get; set; }
    }

    public class GroupNetworkViewModel : BaseViewModel<Group>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public int TotalSession { get; set; }
        public List<SessionNetworkViewModel> Sessions { get; set; }

    }

    public class GroupInSyncData : BaseViewModel<Group>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int MaxSessionCount { get; set; }
        public DateTime DateTimeCreated { get; set; }
    }
}

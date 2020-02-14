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
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public ICollection<AttendeeViewModel> Attendees { get; set; }
        public ICollection <SessionViewModel> Sessions { get; set; }
    }
}

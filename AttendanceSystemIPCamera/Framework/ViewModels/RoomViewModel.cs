using AttendanceSystemIPCamera.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class RoomViewModel : BaseViewModel<Room>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CameraConnectionString { get; set; }
    }
}

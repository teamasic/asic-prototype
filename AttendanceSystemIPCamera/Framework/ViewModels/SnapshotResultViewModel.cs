using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class SnapshotResultViewModel
    {
        // list of codes of recognized attendees ['SE12345', 'SE12346',...]
        public ICollection<string> Codes { get; set; } = new List<string>();
        // list of images of unknown attendees ['1.jpg', '2.jpg',...]
        public ICollection<string> Unknowns { get; set; } = new List<string>();
    }
}

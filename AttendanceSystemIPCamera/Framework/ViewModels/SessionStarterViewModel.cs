﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class CreateSessionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int RoomId { get; set; }
        public string GroupCode { get; set; }
    }
}

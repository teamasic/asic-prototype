using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.GlobalStates
{
    public class GlobalState
    {
        public int CurrentActiveSession { get; set; } = -1; // -1 means no active session
    }
}

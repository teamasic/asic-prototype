using System;
using System.Collections.Generic;
using System.Text;

namespace AttendanceSystemIPCamera.Services.NetworkService
{
    [Serializable]
    public class AESKey
    {
        public AESKey(byte[] key, byte[] iV)
        {
            Key = key;
            IV = iV;
        }

        public byte[] Key { get; set; }
        public byte[] IV { get; set; }
    }
}

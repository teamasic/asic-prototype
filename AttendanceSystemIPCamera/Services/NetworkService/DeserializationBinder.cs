using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Services.NetworkService
{
    sealed class AESKeyDeserializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type typeToDeserialize = null;

            string exeAssembly = Assembly.GetExecutingAssembly().FullName;

            typeToDeserialize = Type.GetType(String.Format("{0}, {1}",typeName, exeAssembly));

            return typeToDeserialize;
        }
    }
}

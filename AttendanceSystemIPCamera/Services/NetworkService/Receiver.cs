using AttendanceSystemIPCamera.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AttendanceSystemIPCamera.Services.NetworkService
{
    public class Receiver
    {
        protected UdpClient localServer = null;

        public Receiver(UdpClient localServer)
        {
            this.localServer = localServer;
        }

        protected  object Receive(ref IPEndPoint remoteHostIP)
        {
            return localServer.Receive(ref remoteHostIP);
        }
        protected byte[] Decode(byte[] encodedMessage) {
            return CryptoUtils.Decrypt(encodedMessage);
        }
        //protected  bool Authenticate(object decodedMessage)
        //{
        //    return true;
        //}

        public object Start(ref IPEndPoint remoteHostIP)
        {
            byte[] encodedMessage = Receive(ref remoteHostIP) as byte[];
            byte[] decodedMessage = Decode(encodedMessage);
            //if (Authenticate(decodedMessage))
            //{
            //    return decodedMessage;
            //}
            return Encoding.UTF8.GetString(decodedMessage);
        }

    }
}

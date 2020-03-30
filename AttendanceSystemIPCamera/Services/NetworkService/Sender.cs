using AttendanceSystemIPCamera.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AttendanceSystemIPCamera.Services.NetworkService
{
    public class Sender
    {
        protected UdpClient localServer = null;

        public Sender(UdpClient localServer)
        {
            this.localServer = localServer;
        }

        protected byte[] Encode(byte[] message)
        {
            return CryptoUtils.Encrypt(message);
        }

        protected void Send(object encodedMessage, IPEndPoint remoteHostIP)
        {
            if (remoteHostIP.Address == IPAddress.Broadcast)
            {
                localServer.EnableBroadcast = true;
            }
            if (encodedMessage is byte[])
            {
                var data = encodedMessage as byte[];
                localServer.Send(data, data.Length, remoteHostIP);
            }
            else throw new InvalidDataException("Data must be byte array");
        }

        public void Start(byte[] message, IPEndPoint remoteHostIP)
        {
            byte[] encodedMessage = Encode(message);
            Send(encodedMessage, remoteHostIP);
        }

    }
}

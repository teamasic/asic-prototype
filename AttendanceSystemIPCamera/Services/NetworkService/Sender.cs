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

        protected object Encode(object plainMessage)
        {
            return plainMessage;
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

        public void Start(object plainMessage, IPEndPoint remoteHostIP)
        {
            object encodedMessage = Encode(plainMessage);
            Send(encodedMessage, remoteHostIP);
        }

    }
}

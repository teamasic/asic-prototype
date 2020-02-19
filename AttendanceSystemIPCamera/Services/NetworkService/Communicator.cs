using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AttendanceSystemIPCamera.Services.NetworkService
{
    public class Communicator
    {
        protected Sender sender;
        protected Receiver receiver;
        protected IPEndPoint remoteHostEP;

        public Communicator(UdpClient localServer, ref IPEndPoint remoteHostEP)
        {
            this.sender = new Sender(localServer);
            this.receiver = new Receiver(localServer);
            this.remoteHostEP = remoteHostEP;
        }

        public Communicator(Sender sender)
        {
            this.sender = sender;
        }

        public Communicator(Receiver receiver)
        {
            this.receiver = receiver;
        }

        public void Send(byte[] plainMessage)
        {
            if(sender != null)
            {
                sender.Start(plainMessage, remoteHostEP);
            }
        }

        public object Receive()
        {
            if(receiver != null)
            {
                return receiver.Start(ref remoteHostEP);
            }
            return null;
        }

    }
}

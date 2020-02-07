using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using AttendanceSystemIPCamera.Utils;

namespace AttendanceSystemIPCamera.Services.NetworkService
{
    public class SupervisorNetworkService
    {
        protected UdpClient localServer;
        protected IPEndPoint remoteHostEP;

        public SupervisorNetworkService()
        {

        }

        public SupervisorNetworkService(UdpClient localServer, IPEndPoint remoteHostEP)
        {
            this.localServer = localServer;
            this.remoteHostEP = remoteHostEP;
        }

        public void Start()
        {
            if (localServer == null)
            {
                localServer = new UdpClient(NetworkUtils.RunningPort);
            }

            while (true)
            {
                var remoteHostEP = new IPEndPoint(IPAddress.Any, 0);
                if(this.remoteHostEP != null)
                {
                    remoteHostEP = this.remoteHostEP;
                }
                Communicator communicator = new Communicator(localServer, ref remoteHostEP);
                
                //receive
                var msg = communicator.Receive();
                Console.WriteLine("Server: " + msg);

                //send
                communicator.Send(Encoding.ASCII.GetBytes("Received"));
            }
        }

    }
}

using AttendanceSystemIPCamera.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AttendanceSystemIPCamera.Services.NetworkService
{
    public class AttendeeNetworkService
    {
        protected UdpClient localServer;
        protected IPEndPoint remoteHostEP;


        public AttendeeNetworkService()
        {

        }

        public AttendeeNetworkService(UdpClient localServer, IPEndPoint remoteHostEP)
        {
            this.localServer = localServer;
            this.remoteHostEP = remoteHostEP;
        }

        public void Start()
        {
            if (localServer == null)    localServer = new UdpClient();

            IPAddress localIp = null;
            IPAddress.TryParse(NetworkUtils.GetLocalIPAddress(), out localIp);

            if (localIp != null)
            {
                string reqMess = "'" + localIp.ToString() + "-SE63159'";
                var remoteHostEP = new IPEndPoint(IPAddress.Broadcast, NetworkUtils.RunningPort);
                if (this.remoteHostEP != null) remoteHostEP = this.remoteHostEP;

                Communicator communicator = new Communicator(localServer, ref remoteHostEP);
                communicator.Send(Encoding.ASCII.GetBytes(reqMess));

                object responseData =  communicator.Receive();
                Console.WriteLine("client: " + responseData);

            }
        }
    }
}

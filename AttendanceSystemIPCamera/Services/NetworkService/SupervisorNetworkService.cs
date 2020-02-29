﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using AttendanceSystemIPCamera.Repositories.UnitOfWork;
using AttendanceSystemIPCamera.Services.AttendeeService;
using AttendanceSystemIPCamera.Services.SessionService;
using AttendanceSystemIPCamera.Utils;
using AutoMapper;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using static AttendanceSystemIPCamera.Framework.Constants;
using AttendanceSystemIPCamera.Framework.AutoMapperProfiles;

namespace AttendanceSystemIPCamera.Services.NetworkService
{

    public class SupervisorNetworkService
    {
        protected UdpClient localServer;
        protected IPEndPoint remoteHostEP;

        private ISessionService sessionService;
        private IAttendeeService attendeeService;
        private IMapper mapper;

        public SupervisorNetworkService(ISessionService sessionService, IAttendeeService attendeeService, IMapper mapper)
        {
            this.sessionService = sessionService;
            this.attendeeService = attendeeService;
            this.mapper = mapper;
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
                if (this.remoteHostEP != null)
                {
                    remoteHostEP = this.remoteHostEP;
                }
                Communicator communicator = new Communicator(localServer, ref remoteHostEP);

                //receive
                var msg = communicator.Receive();
                //Console.WriteLine("Server: " + msg);
                var attendanceData = ProcessRequest(msg);

                //send
                var repsonse = JsonConvert.SerializeObject(attendanceData);
                communicator.Send(Encoding.UTF8.GetBytes(repsonse));
            }
        }

        public AttendanceNetworkViewModel ProcessRequest(object msg)
        {
            (bool success, Attendee attendee) = ValidateMessage(msg.ToString());
            var attendanceData = new AttendanceNetworkViewModel()
            {
                Success = success
            };
            
            if (success)
            {
                attendanceData.AttendeeCode = attendee.Code;
                attendanceData.AttendeeName = attendee.Name;
                var groupIds = attendee.AttendeeGroups.Select(ag => ag.GroupId).ToList();
                attendanceData.Groups = GetGroupNetworkViewModels(attendee);
            }
            return attendanceData;
        }

        private (bool success, Attendee attendee) ValidateMessage(string message)
        {
            try
            {
                var networkData = JsonConvert.DeserializeObject<NetworkMessageViewModel>(message.ToString());
                var loginViewModel = networkData.Message;
                Attendee attendee = null;
                switch (loginViewModel.LoginMethod)
                {
                    case Constant.LOGIN_BY_USERNAME_PASSWORD:
                        break;
                    case Constant.LOGIN_BY_FACE:
                        break;
                    case Constant.GET_DATA_BY_ATTENDEE_CODE:
                        attendee = attendeeService.GetByAttendeeCodeForNetwork(loginViewModel.AttendeeCode);
                        break;
                }
                if (attendee != null)
                    return (true, attendee);
            }
            catch (Exception e)
            {
            }
            return (false, null);
        }

        private List<GroupNetworkViewModel> GetGroupNetworkViewModels(Attendee attendee)
        {
            var groupNetworks = mapper.ProjectTo<Group, GroupNetworkViewModel>(attendee.Groups).ToList();
            groupNetworks.RemoveAll(g => g.Sessions.Count == 0);
            groupNetworks.ForEach(groupSession =>
            {
                groupSession.Sessions.RemoveAll(s => s.Records.Count == 0);
                groupSession.Sessions.ForEach(session =>
                {
                    session.Records.RemoveAll(r => r.AttendeeId != attendee.Id);
                });
            });
            return groupNetworks.ToList();
        }
    }
}

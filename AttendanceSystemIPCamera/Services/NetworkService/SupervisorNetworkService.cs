using System;
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
using AttendanceSystemIPCamera.Services.ChangeRequestService;
using AttendanceSystemIPCamera.Services.SessionService;
using AttendanceSystemIPCamera.Utils;
using AutoMapper;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using static AttendanceSystemIPCamera.Framework.Constants;
using AttendanceSystemIPCamera.Framework.AutoMapperProfiles;
using Microsoft.Extensions.DependencyInjection;

namespace AttendanceSystemIPCamera.Services.NetworkService
{

    public class SupervisorNetworkService
    {
        protected UdpClient localServer;
        protected IPEndPoint remoteHostEP;

        private IChangeRequestService changeRequestService;
        private ISessionService sessionService;
        private IAttendeeService attendeeService;
        private IMapper mapper;

        private IServiceScopeFactory serviceScopeFactory;

        private Communicator communicator;

        public SupervisorNetworkService(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
        {
            this.serviceScopeFactory = serviceScopeFactory;
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
                communicator = new Communicator(localServer, ref remoteHostEP);

                //receive
                var msg = communicator.Receive()?.ToString();

                using (var scope = serviceScopeFactory.CreateScope())
                {
                    this.sessionService = scope.ServiceProvider.GetService<ISessionService>();
                    this.attendeeService = scope.ServiceProvider.GetService<IAttendeeService>();
                    this.changeRequestService = scope.ServiceProvider.GetService<IChangeRequestService>();

                    try
                    {
                        var networkRequest = JsonConvert.DeserializeObject<NetworkRequest<object>>(msg);
                        switch (networkRequest.Route)
                        {
                            case NetworkRoute.LOGIN:
                                Login(msg);
                                break;
                            case NetworkRoute.REFRESH_ATTENDANCE_DATA:
                                Refresh(msg);
                                break;
                            case NetworkRoute.CHANGE_REQUEST:
                                ChangeRequest(msg);
                                break;
                            default:
                                //throw new exception
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        var trace = e.StackTrace;
                    }
                }
            }
        }

        private void Login(string msg)
        {
            var networkData = JsonConvert.DeserializeObject<NetworkRequest<LoginViewModel>>(msg);
            (bool success, Attendee attendee) = ValidateAttendee(networkData.Request);
            var attendanceData = new AttendanceNetworkViewModel()
            {
                Success = success
            };
            if (success)
            {
                attendanceData.AttendeeCode = attendee.Code;
                attendanceData.AttendeeName = attendee.Name;
                var groupCodes = attendee.AttendeeGroups.Select(ag => ag.GroupCode).ToList();
                attendanceData.Groups = GetGroupNetworkViewModels(attendee);
            }
            //send
            Send(attendanceData);
        }

        private void Send(object data)
        {
            var repsonse = JsonConvert.SerializeObject(data);
            communicator.Send(Encoding.UTF8.GetBytes(repsonse));
        }

        private void Refresh(string msg)
        {
            Login(msg);
        }

        private async void ChangeRequest(string msg)
        {
            var networkData = JsonConvert.DeserializeObject<NetworkRequest<CreateChangeRequestViewModel>>(msg);
            var createChangeRequest = networkData.Request;
            try
            {
                var newChangeRequest = await changeRequestService.Add(createChangeRequest);
                newChangeRequest.Record = null;
                Send(newChangeRequest);
            }
            catch (Exception)
            {
                Send(ErrorMessage.CREATE_REQUEST_ERROR);
            }
        }

        private (bool success, Attendee attendee) ValidateAttendee(LoginViewModel loginViewModel)
        {
            try
            {
                Attendee attendee = null;
                switch (loginViewModel.LoginMethod)
                {
                    case Constant.LOGIN_BY_USERNAME_PASSWORD:
                        break;
                    case Constant.LOGIN_BY_FACE:
                        attendee = attendeeService.GetByAttendeeFaceForNetwork(loginViewModel.FaceData);
                        break;
                    case Constant.GET_DATA_BY_ATTENDEE_CODE:
                        attendee = attendeeService.GetByAttendeeCodeForNetwork(loginViewModel.AttendeeCode);
                        break;
                }
                if (attendee != null)
                    return (true, attendee);
            }
            catch (Exception)
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
                    session.Records.RemoveAll(r => r.AttendeeCode != attendee.Code);
                });
                groupSession.Sessions.RemoveAll(s => s.Records.Count == 0);
            });
            return groupNetworks.ToList();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using Microsoft.EntityFrameworkCore;
using AttendanceSystemIPCamera.Repositories.UnitOfWork;
using AttendanceSystemIPCamera.Services.BaseService;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using AttendanceSystemIPCamera.Repositories;
using Microsoft.AspNetCore.SignalR;
using System.Timers;

namespace AttendanceSystemIPCamera.Services.RecordService
{
    public interface IRealTimeService
    {
        public Task MarkAttendeeAsPresent(string attendeeCode);
        public Task MarkAttendeeAsUnknown(string imageName);
        public Task SessionEnded(int sessionId);
        public Task NewChangeRequestAdded();
    }

    public class HubMethods
    {
        public static string ATTENDEE_PRESENTED = "attendeePresented";
        public static string ATTENDEE_UNKNOWN = "attendeeUnknown";
        public static string SESSION_ENDED = "sessionEnded";
        public static string KEEP_ALIVE = "keepAlive";
        public static string NEW_CHANGE_REQUEST = "newChangeRequest";
    }

    public class RealTimeService : Hub, IRealTimeService
    {
        private readonly IHubContext<RealTimeService> hubContext;
        private readonly Timer timer;
        public RealTimeService(IHubContext<RealTimeService> hubContext)
        {
            this.hubContext = hubContext;
            timer = new Timer(TimeSpan.FromSeconds(30).TotalMilliseconds);
            timer.Elapsed += async (source, e) =>
            {
                await KeepAlive();
            };
            timer.AutoReset = true;
        }
        public async Task MarkAttendeeAsPresent(string attendeeCode)
        {
            await hubContext.Clients.All.SendAsync(HubMethods.ATTENDEE_PRESENTED, attendeeCode);
        }
        public async Task MarkAttendeeAsUnknown(string imageName)
        {
            await hubContext.Clients.All.SendAsync(HubMethods.ATTENDEE_UNKNOWN, imageName);
        }
        public async Task NewChangeRequestAdded()
        {
            await hubContext.Clients.All.SendAsync(HubMethods.NEW_CHANGE_REQUEST);
        }

        public async Task SessionEnded(int sessionId)
        {
            await hubContext.Clients.All.SendAsync(HubMethods.SESSION_ENDED, sessionId);
        }

        public async Task KeepAlive()
        {
            await hubContext.Clients.All.SendAsync(HubMethods.KEEP_ALIVE);
        }

        public override Task OnConnectedAsync()
        {
            timer.Start();
            return Task.CompletedTask;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            timer?.Stop();
            return Task.CompletedTask;
        }
    }
}

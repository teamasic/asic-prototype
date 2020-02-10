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
using AttendanceSystemIPCamera.Framework.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace AttendanceSystemIPCamera.Services.RecordService
{
    public interface IRealTimeService
    {
        public Task MarkAttendeeAsPresent(int attendeeId);
    }

    public class HubMethods
    {
        public static string ATTENDEE_PRESENTED = "attendeePresented";
    }

    public class RealTimeService : Hub, IRealTimeService
    {
        private readonly IHubContext<RealTimeService> hubContext;
        public RealTimeService(IHubContext<RealTimeService> hubContext)
        {
            this.hubContext = hubContext;
        }
        public async Task MarkAttendeeAsPresent(int attendeeId)
        {
            await hubContext.Clients.All.SendAsync(HubMethods.ATTENDEE_PRESENTED, attendeeId);
        }
    }
}

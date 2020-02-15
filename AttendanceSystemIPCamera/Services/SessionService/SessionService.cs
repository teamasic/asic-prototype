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

namespace AttendanceSystemIPCamera.Services.SessionService
{
    public interface ISessionService : IBaseService<Session>
    {
        bool isSessionRunning();
        Task<Session> getActiveSession();
    }

    public class SessionService: BaseService<Session>, ISessionService
    {
        private readonly ISessionRepository sessionRepository;
        private readonly IGroupRepository groupRepository;
        public SessionService(MyUnitOfWork unitOfWork) : base(unitOfWork)
        {
            sessionRepository = unitOfWork.SessionRepository;
            groupRepository = unitOfWork.GroupRepository;
        }
        public async Task Add(TakeAttendanceViewModel viewModel)
        {
            var group = await groupRepository.GetById(viewModel.GroupId);
            group.Sessions.Add(new Session
            {
                Group = group,
                StartTime = DateTime.UtcNow,
                Duration = viewModel.Duration
            });
            groupRepository.Update(group);
            unitOfWork.Commit();
        }

        public async Task<Session> getActiveSession()
        {
            return await sessionRepository.GetActiveSession();
        }

        public bool isSessionRunning()
        {
            return sessionRepository.isSessionRunning();
        }
    }
}

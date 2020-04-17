using AttendanceSystemIPCamera.Framework.GlobalStates;
using AttendanceSystemIPCamera.Models;
using AttendanceSystemIPCamera.Services.GroupService;
using AttendanceSystemIPCamera.Services.SessionService;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Repositories.UnitOfWork
{
    public class MyUnitOfWork: UnitOfWork
    {
        public MyUnitOfWork(DbContext dbContext, GlobalState globalState) : base(dbContext)
        {
            this.globalState = globalState;
        }
        public IRepository<T> GetRepository<T>() where T: class
        {
            return new Repository<T>(DbContext);
        }

        #region Repository
        private GlobalState globalState;
        private IGroupRepository groupRepository;
        private ISessionRepository sessionRepository;
        private IRecordRepository recordRepository;
        private IAttendeeRepository attendeeRepository;
        private IRoomRepository roomRepository;
        private IAttendeeGroupRepository attendeeGroupRepository;
        private IChangeRequestRepository changeRequestRepository;

        public IGroupRepository GroupRepository
        {
            get
            {
                if (groupRepository == null)
                {
                    groupRepository = new GroupRepository(DbContext);
                }
                return groupRepository;
            }
        }
        public ISessionRepository SessionRepository
        {
            get
            {
                if (sessionRepository == null)
                {
                    sessionRepository = new SessionRepository(DbContext, globalState);
                }
                return sessionRepository;
            }
        }
        public IRecordRepository RecordRepository
        {
            get
            {
                if (recordRepository == null)
                {
                    recordRepository = new RecordRepository(DbContext);
                }
                return recordRepository;
            }
        }
        public IAttendeeRepository AttendeeRepository
        {
            get
            {
                if (attendeeRepository == null)
                {
                    attendeeRepository = new AttendeeRepository(DbContext);
                }
                return attendeeRepository;
            }
        }
        public IRoomRepository RoomRepository
        {
            get
            {
                if (roomRepository == null)
                {
                    roomRepository = new RoomRepository(DbContext);
                }
                return roomRepository;
            }
        }

        public IAttendeeGroupRepository AttendeeGroupRepository
        {
            get
            {
                if(attendeeGroupRepository == null)
                {
                    attendeeGroupRepository = new AttendeeGroupRepository(DbContext);
                }
                return attendeeGroupRepository;
            }
        }

        public IChangeRequestRepository ChangeRequestRepository
        {
            get
            {
                if (changeRequestRepository == null)
                {
                    changeRequestRepository = new ChangeRequestRepository(DbContext);
                }
                return changeRequestRepository;
            }
        }
        #endregion

    }
}

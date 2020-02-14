﻿using AttendanceSystemIPCamera.Models;
using AttendanceSystemIPCamera.Services.GroupService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Repositories.UnitOfWork
{
    public class MyUnitOfWork: UnitOfWork
    {
        public MyUnitOfWork(DbContext dbContext) : base(dbContext)
        {
        }
        public IRepository<T> GetRepository<T>() where T: class, BaseEntity
        {
            return new Repository<T>(DbContext);
        }

        private IGroupRepository groupRepository;
        private ISessionRepository sessionRepository;
        private IRecordRepository recordRepository;
        private IAttendeeRepository attendeeRepository;
        private IAttendeeGroupRepository attendeeGroupRepository;

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
                    sessionRepository = new SessionRepository(DbContext);
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
    }
}

using AttendanceSystemIPCamera.Models;
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AttendanceSystemIPCamera.Repositories
{
    public interface ISessionRepository : IRepository<Session>
    {
        bool isSessionRunning();
        Task<Session> GetActiveSession();
    }
    public class SessionRepository : Repository<Session>, ISessionRepository
    {
        public SessionRepository(DbContext context) : base(context)
        {
        }

        public async Task<Session> GetActiveSession()
        {
            return await dbSet
                .Include(s => s.Group)
                .ThenInclude(g => g.AttendeeGroups)
                .FirstOrDefaultAsync(s => s.Active == true);
        }

        public bool isSessionRunning()
        {
            return dbSet.Any(s => s.Active == true);
        }
    }
}

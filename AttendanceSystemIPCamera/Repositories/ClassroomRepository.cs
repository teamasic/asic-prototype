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
    public interface IClassroomRepository : IRepository<Classroom>
    {
        public Task<Classroom> GetClassroomByName(string name);
    }
    public class ClassroomRepository : Repository<Classroom>, IClassroomRepository
    {
        public ClassroomRepository(DbContext context) : base(context)
        {
        }

        public async Task<Classroom> GetClassroomByName(string name)
        {
            return await dbSet.FirstOrDefaultAsync(c => c.Name.Equals(name));
        }
    }
}

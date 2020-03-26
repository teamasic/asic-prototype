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
    public interface IRoomRepository : IRepository<Room>
    {
        public Task<Room> GetRoomByName(string name);
        public void ClearAllRooms();
        public Task AddRooms(IEnumerable<Room> rooms);
    }
    public class RoomRepository : Repository<Room>, IRoomRepository
    {
        public RoomRepository(DbContext context) : base(context)
        {
        }

        public async Task<Room> GetRoomByName(string name)
        {
            return await dbSet.FirstOrDefaultAsync(c => c.Name.Equals(name));
        }
        public void ClearAllRooms()
        {
            dbSet.RemoveRange(context.Set<Room>());
        }
        public async Task AddRooms(IEnumerable<Room> rooms)
        {
            await dbSet.AddRangeAsync(rooms);
        }
    }
}

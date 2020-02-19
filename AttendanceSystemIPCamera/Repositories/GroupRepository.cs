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
    public interface IGroupRepository: IRepository<Group>
    {
        public Task<PaginatedList<Group>> GetAll(GroupSearchViewModel groupSearchViewModel);
        Task<bool> CheckAttendeeExistedInGroup(int id, string attendeeCode);
    }
    public class GroupRepository : Repository<Group>, IGroupRepository
    {
        public GroupRepository(DbContext context) : base(context)
        {
        }
        private Func<IQueryable<Group>, IOrderedQueryable<Group>> Order(OrderBy orderBy)
        {
            switch (orderBy)
            {
                case OrderBy.Name:
                    return groups => groups.OrderBy(g => g.Name);
                case OrderBy.DateCreated:
                default:
                    return groups => groups.OrderByDescending(g => g.DateTimeCreated);
            }
        }
        public async Task<PaginatedList<Group>> GetAll(GroupSearchViewModel groupSearchViewModel)
        {
            var orderFunction = Order(groupSearchViewModel.OrderBy);
            var query = dbSet
                .Include(g => g.AttendeeGroups)
                    .ThenInclude(ag => ag.Attendee)
                .Include(g => g.Sessions)
                .Where(g => g.Name.ToLower().Contains(groupSearchViewModel.NameContains.ToLower()));
            query = orderFunction(query);
            return await PaginatedList<Group>.CreateAsync(query, groupSearchViewModel.Page, groupSearchViewModel.PageSize);
        }
        //public new async Task<Group> GetById(object id)
        //{
        //    return await dbSet.Include(g => g.Sessions).FirstOrDefaultAsync(g => id.Equals(g.Id));
        //}

        public async Task<bool> CheckAttendeeExistedInGroup(int id, string attendeeCode)
        {
            var group = await dbSet.Include(g => g.AttendeeGroups).ThenInclude(a => a.Attendee).FirstOrDefaultAsync(g => g.Id == id);
            return group.Attendees.Any(a => a.Code.Equals(attendeeCode));
                
        }
    }
}

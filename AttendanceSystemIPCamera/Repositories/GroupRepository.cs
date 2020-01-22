using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttendanceSystemIPCamera.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AttendanceSystemIPCamera.Repositories
{
    public class GroupRepository : IRepository<Group>
    {
        public Task<Group> Add(Group entity)
        {
            throw new NotImplementedException();
        }

        public Task<Group> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Group> Get(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Group>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<Group> Update(Group entity)
        {
            throw new NotImplementedException();
        }
    }
}

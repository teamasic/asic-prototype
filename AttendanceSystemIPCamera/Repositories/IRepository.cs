using AttendanceSystemIPCamera.Models;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Repositories
{
    public interface IRepository<T> : IDisposable where T : class, BaseEntity
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(object id);

        Task Add(T entity);
        Task Add(IEnumerable<T> entities);


        void Delete(T entity);
        void Delete(object id);
        void Delete(IEnumerable<T> entities);


        void Update(T entity);
        void Update(IEnumerable<T> entities);
    }
}

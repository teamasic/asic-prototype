using AttendanceSystemIPCamera.Framework.ViewModels;
using AttendanceSystemIPCamera.Models;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Services.BaseService
{
    public interface IBaseService<T>
        where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(object id);

        Task<T> Add(T entity);
        Task Add(IEnumerable<T> entities);


        void Delete(T entity);
        void Delete(object id);
        void Delete(IEnumerable<T> entities);


        void Update(T entity);
        void Update(IEnumerable<T> entities);
    }
}
using AttendanceSystemIPCamera.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Services.BaseService
{
    public interface IBaseService<TEntity, TViewModel>
        where TEntity : class, new()
        where TViewModel : BaseViewModel<TEntity>, new()
    {
        TViewModel FindById(int id);
        Task<TViewModel> FindByIdAsync(int id);
        TViewModel FindById<TKey>(TKey id);
        Task<TViewModel> FindByIdAsync<TKey>(TKey id);

        IQueryable<TViewModel> GetAll();
        IQueryable<TViewModel> Get(Expression<Func<TEntity, bool>> predicate);

        TViewModel FirstOrDefault();
        TViewModel FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        Task<TViewModel> FirstOrDefaultAsync();
        TViewModel Create(TViewModel viewModel);
        Task<TViewModel> CreateAsync(TViewModel viewModel);
        TViewModel Update(TViewModel viewModel);
        Task<TViewModel> UpdateAsync(TViewModel viewModel);
        void DeleteByObj(TViewModel viewModel);
        Task DeleteByObjAsync(TViewModel viewModel);
        void DeleteByKey<TKey>(TKey id);
        Task DeleteByKeyAsync<TKey>(TKey id);
    }
}
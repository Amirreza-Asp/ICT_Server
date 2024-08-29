using Domain.Dtos.Shared;
using Domain.Queries.Shared;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TDto> FirstOrDefaultAsync<TDto>(
          Expression<Func<TEntity, bool>> filters = null,
          CancellationToken cancellationToken = default) where TDto : class;

        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter = null,
                Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                CancellationToken cancellationToken = default,
                bool isTracking = false);


        Task<ListActionResult<TDto>> GetAllAsync<TDto>(
           GridQuery query,
           CancellationToken cancellationToken = default) where TDto : class;

        Task<ListActionResult<TDto>> GetAllAsync<TDto>(
          GridQuery query,
          Expression<Func<TEntity, bool>> filters = null,
          CancellationToken cancellationToken = default) where TDto : class;


        Task<List<TDto>> GetAllAsync<TDto>(
            Expression<Func<TEntity, bool>> filters = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            CancellationToken cancellationToken = default);

        Task<List<TDto>> GetAllAsync<TDto>(
            GridQuery query,
            Expression<Func<TEntity, bool>> filters = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            CancellationToken cancellationToken = default);

        Task<int> CountAsync(CancellationToken cancellationToken);

        void Create(TEntity entity);
        void Update(TEntity entity);

        void Remove(TEntity entity);
        void Remove(object id);

        Task<bool> SaveAsync(CancellationToken cancellationToken = default);
    }
}

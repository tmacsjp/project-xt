using Microsoft.EntityFrameworkCore;
using OA.Core;
using OA.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OA.Infrastructure
{

    public class BaseRepository<TDbContext, TEntity> : IBaseRepository<TEntity>
        where TEntity : class
        where TDbContext :AdoDbContext
    {
        protected TDbContext DbContext;
        protected DbSet<TEntity> Table;

        public BaseRepository(TDbContext dbContext)
        {
            DbContext = dbContext;
            Table = dbContext.Set<TEntity>();
        }

        public Task<int> SaveChangesAsync() => DbContext.SaveChangesAsync();
        public int SaveChanges() => DbContext.SaveChanges();

        public void ReAttach<T>(T entity) where T : class
        {
            DbContext.Attach(entity).State = EntityState.Unchanged;
        }

        public void Add(TEntity entity, bool autoSaveChange = true)
        {
            Table.Add(entity);
            if (autoSaveChange)
                DbContext.SaveChanges();
        }
        public void Delete(TEntity entity, bool autoSaveChange = true)
        {
            Table.Attach(entity).State = EntityState.Deleted;
            if (autoSaveChange)
                DbContext.SaveChanges();
        }

        public void Update(TEntity entity, bool autoSaveChange = true)
        {
            Table.Attach(entity).State = EntityState.Modified;
            if (autoSaveChange)
                DbContext.SaveChanges();
        }



        public async Task<int> AddAsync(TEntity entity, bool autoSaveChange = true)
        {
            Table.Add(entity);
            if (autoSaveChange)
                return await DbContext.SaveChangesAsync();
            return 0;
        }
        public async Task<int> DeleteAsync(TEntity entity, bool autoSaveChange = true)
        {
            Table.Attach(entity).State = EntityState.Deleted;
            if (autoSaveChange)
                return await SaveChangesAsync();
            return 0;
        }


        public async Task<int> UpdateAsync(TEntity entity, bool autoSaveChange = true)
        {
            Table.Attach(entity).State = EntityState.Modified;
            if (autoSaveChange)
                return await DbContext.SaveChangesAsync();
            return 0;
        }




        public void Add<T>(T entity, bool autoSaveChange = true) where T : class
        {
            DbContext.Set<T>().Add(entity);
            if (autoSaveChange)
                DbContext.SaveChanges();
        }

        public void Delete<T>(T entity, bool autoSaveChange = true) where T : class
        {
            DbContext.Set<T>().Attach(entity).State = EntityState.Deleted;
            if (autoSaveChange)
                DbContext.SaveChanges();
        }

        public void Update<T>(T entity, bool autoSaveChange = true) where T : class
        {
            DbContext.Set<T>().Attach(entity).State = EntityState.Modified;
            if (autoSaveChange)
                DbContext.SaveChanges();
        }

        public async Task<int> AddAsync<T>(T entity, bool autoSaveChange = true)
            where T : class
        {
            DbContext.Set<T>().Add(entity);
            if (autoSaveChange)
                return await DbContext.SaveChangesAsync();
            return 0;
        }

        public async Task<int> DeleteAsync<T>(T entity, bool autoSaveChange = true)
            where T : class
        {
            DbContext.Set<T>().Attach(entity).State = EntityState.Deleted;
            if (autoSaveChange)
                return await DbContext.SaveChangesAsync();
            return 0;
        }


        public async Task<int> UpdateAsync<T>(T entity, bool autoSaveChange = true)
            where T : class
        {
            DbContext.Set<T>().Attach(entity).State = EntityState.Modified;
            if (autoSaveChange)
              return await  DbContext.SaveChangesAsync();
            return 0;
        }


        /// <summary>
        /// 获取单条数据
        /// </summary>
        /// <param name="expWhere">查询条件表达式</param>
        /// <param name="noTracking">是否跟踪实体</param>
        public Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> expWhere, bool noTracking = false)
        {
            var query = Table.AsQueryable();
            if (noTracking)
                query = query.AsNoTracking();
            return query.FirstOrDefaultAsync(expWhere);
        }

        public Task<T?> GetAsync<T>(Expression<Func<T, bool>> expWhere, bool noTracking=false)
             where T : class
        {
            var query = DbContext.Set<T>().AsQueryable();
            if (noTracking)
                query = query.AsNoTracking();
            return query.FirstOrDefaultAsync(expWhere);
        }

        /// <summary>
        /// 获取单条数据
        /// </summary>
        /// <param name="expWhere">查询条件表达式</param>
        /// <param name="noTracking">是否跟踪实体</param>
        public TEntity? Get(Expression<Func<TEntity, bool>> expWhere, bool noTracking = false)
        {
            var query = Table.AsQueryable();
            if (noTracking)
                query = query.AsNoTracking();
            return query.FirstOrDefault(expWhere);
        }

        public T Get<T>(Expression<Func<T, bool>> expWhere, bool noTracking = false)
             where T : class
        {
            var query = DbContext.Set<T>().AsQueryable();
            if (noTracking)
                query = query.AsNoTracking();
            return query.FirstOrDefault(expWhere);
        }

        /// <summary>
        /// 获取单条数据
        /// </summary>
        /// <param name="expWhere">查询条件表达式</param>
        /// <param name="noTracking">是否跟踪实体</param>
        public Task<List<TEntity>> ListAsync(Expression<Func<TEntity, bool>> expWhere, bool noTracking = false)
        {
            var query = Table.AsQueryable();
            if (noTracking)
                query = query.AsNoTracking();
            return query.Where(expWhere).ToListAsync();
        }

        /// <summary>
        /// 获取单条数据
        /// </summary>
        /// <param name="expWhere">查询条件表达式</param>
        /// <param name="noTracking">是否跟踪实体</param>
        public Task<List<T>> ListAsync<T>(Expression<Func<T, bool>> expWhere, bool noTracking = false)
           where T : class
        {
            var query = DbContext.Set<T>().AsQueryable();
            if (noTracking)
                query = query.AsNoTracking();
            return query.Where(expWhere).ToListAsync();
        }


        /// <summary>
        /// 获取单条数据
        /// </summary>
        /// <param name="expWhere">查询条件表达式</param>
        /// <param name="noTracking">是否跟踪实体</param>
        public List<TEntity> List(Expression<Func<TEntity, bool>> expWhere, bool noTracking = false)
        {
            var query = Table.AsQueryable();
            if (noTracking)
                query = query.AsNoTracking();
            return query.Where(expWhere).ToList();
        }

        /// <summary>
        /// 获取单条数据
        /// </summary>
        /// <param name="expWhere">查询条件表达式</param>
        /// <param name="noTracking">是否跟踪实体</param>
        public List<T> List<T>(Expression<Func<T, bool>> expWhere, bool noTracking = false)
           where T : class
        {
            var query = DbContext.Set<T>().AsQueryable();
            if (noTracking)
                query = query.AsNoTracking();
            return query.Where(expWhere).ToList();
        }


        public Task<int> CountAsync(Expression<Func<TEntity, bool>> expWhere, bool noTracking = false)
        {
            var query = Table.AsQueryable();
            if (noTracking)
                query = query.AsNoTracking();
            return query.CountAsync(expWhere);
        }

        public Task<int> CountAsync<T>(Expression<Func<T, bool>> expWhere, bool noTracking = false) where T : class
        {
            var query = DbContext.Set<T>().AsQueryable();
            if (noTracking)
                query = query.AsNoTracking();
            return query.CountAsync(expWhere);
        }


        public int Count(Expression<Func<TEntity, bool>> expWhere, bool noTracking = false)
        {
            var query = Table.AsQueryable();
            if (noTracking)
                query = query.AsNoTracking();
            return query.Count(expWhere);
        }

        public int Count<T>(Expression<Func<T, bool>> expWhere, bool noTracking = false) where T : class
        {
            var query = DbContext.Set<T>().AsQueryable();
            if (noTracking)
                query = query.AsNoTracking();
            return query.Count(expWhere);
        }

        public void BeginTransaction()
        {
            DbContext.AdoBeginTransaction();
        }

        public void CommitTransaction()
        {
            DbContext.AdoCommit();
        }

        public void RollbackTransaction()
        {
            DbContext.AdoRollback();
        }
    }
}

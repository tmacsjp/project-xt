using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OA.Domain
{

    public interface IBaseRepository<T> where T : class
    {
        Task<int> SaveChangesAsync();
        int SaveChanges();

        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();

        void ReAttach<T>(T entity) where T : class;
        void Add(T entity, bool autoSaveChange=true);
        void Update(T entity, bool autoSaveChange = true);
        void Delete(T entity, bool autoSaveChange = true);
        void Add<T>(T entity, bool autoSaveChange = true) where T : class;
        void Update<T>(T entity, bool autoSaveChange = true) where T : class;
        void Delete<T>(T entity, bool autoSaveChange = true) where T : class;

        Task<int> AddAsync(T entity, bool autoSaveChange = true);
        Task<int> UpdateAsync(T entity, bool autoSaveChange = true);
        Task<int> DeleteAsync(T entity, bool autoSaveChange = true);


       Task<int> AddAsync<T>(T entity,bool autoSaveChange=true) where T : class;
        Task<int> UpdateAsync<T>(T entity, bool autoSaveChange = true) where T : class;
        Task<int> DeleteAsync<T>(T entity, bool autoSaveChange = true) where T : class;

        Task<T?> GetAsync(Expression<Func<T, bool>> expWhere, bool noTracking = false); 
        Task<T?> GetAsync<T>(Expression<Func<T, bool>> expWhere, bool noTracking = false) where T : class;

        T? Get(Expression<Func<T, bool>> expWhere, bool noTracking = false);
        T? Get<T>(Expression<Func<T, bool>> expWhere, bool noTracking = false) where T : class;

        Task<int> CountAsync(Expression<Func<T, bool>> expWhere, bool noTracking = false);
        Task<int> CountAsync<T>(Expression<Func<T, bool>> expWhere, bool noTracking = false) where T : class;
        int Count(Expression<Func<T, bool>> expWhere, bool noTracking = false);
        int Count<T>(Expression<Func<T, bool>> expWhere, bool noTracking = false) where T : class;


        Task<List<T>> ListAsync(Expression<Func<T, bool>> expWhere, bool noTracking = false);
        Task<List<T>> ListAsync<T>(Expression<Func<T, bool>> expWhere, bool noTracking = false) where T : class;
        List<T> List(Expression<Func<T, bool>> expWhere, bool noTracking = false);
        List<T> List<T>(Expression<Func<T, bool>> expWhere, bool noTracking = false) where T : class;


    }
}

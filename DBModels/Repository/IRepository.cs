using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace MediaPlus.DBModels.Repository
{

    public interface IRepository<T>
    {
        IQueryable<T> GetAll();
        void Add(T entity);
        IEnumerable<T> GetAllEntities();
        IQueryable<T> EntitiesIQueryable();
        IEnumerable<T> GetEntitiesToShow(int pageNo, int pageSize, int currentPage, Expression<Func<T, bool>> wherePredict, Expression<Func<T, int>> orderByPredict);
        T GetEntity(int entityID);
        T GetEntity(string entityID);
        T GetEntityByParamter(Expression<Func<T, bool>> wherePredict);
        IEnumerable<T> GetListByParamter(Expression<Func<T, bool>> wherePredict);
        int GetNumberOfEntities();
        IQueryable<T> GetResultBySqlProcedure(string query, params object[] parameters);
        void Remove(int entityID);
        void Remove(string entityID);
        void RemoveByWhereClause(Expression<Func<T, bool>> wherePredict);
        void RemoveRangeByWhereClause(Expression<Func<T, bool>> wherePredict);
        void Update(T entity);
        void UpdateByWhereClause(Expression<Func<T, bool>> wherePredict, Action<T> forEachPredict);
        T FirstOrDefault(Expression<Func<T, bool>> predicate);

    }

}

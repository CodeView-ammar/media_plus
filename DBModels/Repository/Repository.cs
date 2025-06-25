using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace MediaPlus.DBModels.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly MediaPlusDbContext _context;
        private readonly DbSet<T> _dbset;

        public Repository(MediaPlusDbContext context)
        {
            _context = context;
            _dbset = _context.Set<T>();
        }

        public void Add(T entity)
        {
            _dbset.Add(entity);
            _context.SaveChanges();
        }

        public IEnumerable<T> GetAllEntities()
        {
            return _dbset.ToList();
        }

        public IQueryable<T> EntitiesIQueryable()
        {
            return _dbset;
        }

        public IEnumerable<T> GetEntitiesToShow(int pageNo, int pageSize, int currentPage, Expression<Func<T, bool>> wherePredict, Expression<Func<T, int>> orderByPredict)
        {
            if (wherePredict != null)
            {
                return _dbset.Where(wherePredict).OrderBy(orderByPredict).ToList();
            }
            else
            {
                return _dbset.OrderBy(orderByPredict).ToList();
            }
        }

        public T GetEntity(int entityID)
        {
            return _dbset.Find(entityID);
        }

        public T GetEntity(string entityID)
        {
            return _dbset.Find(entityID);
        }

        public T GetEntityByParamter(Expression<Func<T, bool>> wherePredict)
        {
            return _dbset.FirstOrDefault(wherePredict);
        }

        public IEnumerable<T> GetListByParamter(Expression<Func<T, bool>> wherePredict)
        {
            return _dbset.Where(wherePredict).ToList();
        }

        public int GetNumberOfEntities()
        {
            return _dbset.Count();
        }

        public IQueryable<T> GetResultBySqlProcedure(string query, params object[] parameters)
        {
            return parameters != null ? _dbset.FromSqlRaw(query, parameters) : _dbset.FromSqlRaw(query);
        }

        public void Remove(int entityID)
        {
            var entity = GetEntity(entityID);
            if (entity != null)
            {
                _dbset.Remove(entity);
                _context.SaveChanges();
            }
        }

        public void Remove(string entityID)
        {
            var entity = GetEntity(entityID);
            if (entity != null)
            {
                _dbset.Remove(entity);
                _context.SaveChanges();
            }
        }

        public void RemoveByWhereClause(Expression<Func<T, bool>> wherePredict)
        {
            var entity = _dbset.FirstOrDefault(wherePredict);
            if (entity != null)
            {
                _dbset.Remove(entity);
                _context.SaveChanges();
            }
        }

        public void RemoveRangeByWhereClause(Expression<Func<T, bool>> wherePredict)
        {
            var entities = _dbset.Where(wherePredict).ToList();
            if (entities.Any())
            {
                _dbset.RemoveRange(entities);
                _context.SaveChanges();
            }
        }

        public void Update(T entity)
        {
            _dbset.Update(entity);
            _context.SaveChanges();
        }
        public IQueryable<T> GetAll()
        {
            return _dbset.AsQueryable();
        }
        public void UpdateByWhereClause(Expression<Func<T, bool>> wherePredict, Action<T> forEachPredict)
        {
            var entities = _dbset.Where(wherePredict).ToList();
            foreach (var entity in entities)
            {
                forEachPredict(entity);
            }
            _context.SaveChanges();
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return _dbset.FirstOrDefault(predicate);
        }
    }
}
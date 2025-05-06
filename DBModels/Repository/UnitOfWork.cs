using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MediaPlus.DBModels.Repository
{
    public class UnitOfWork 
    {
        private readonly MediaPlusDbContext DbEntity = new();
        public IRepository<T> GetRepositoryInstance<T>() where T : class
        {
            return new Repository<T>(DbEntity);
        }
        public MediaPlusDbContext GetDBInstance() 
        {
            return DbEntity;
        }

        public void SaveChages()
        {
            DbEntity.SaveChangesAsync();
        }
    }
}

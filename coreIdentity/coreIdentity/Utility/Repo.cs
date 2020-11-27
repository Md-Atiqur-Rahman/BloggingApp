using coreIdentity.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coreIdentity.Utility
{
    public class Repo<T> : Irepo<T> where T : class
    {
        private readonly dbIdentity _db;
        private DbSet<T> table;

        public Repo(dbIdentity db)
        {
            this._db = db;
            table = _db.Set<T>();

        }
        public int Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll()
        {
            throw new NotImplementedException();
        }

        public T GetByID()
        {
            throw new NotImplementedException();
        }

        public int Save(T obj)
        {
            table.Add(obj);
            return _db.SaveChanges();
        }

        public int Update(T obj)
        {
            throw new NotImplementedException();
        }
    }
}

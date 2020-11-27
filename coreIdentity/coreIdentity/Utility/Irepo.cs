using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coreIdentity.Utility
{
   public interface Irepo<T> where T:class
    {
        IEnumerable<T> GetAll();
        T GetByID();
        int Save(T obj);
        int Update(T obj);
        int Delete(int id);

    }
}

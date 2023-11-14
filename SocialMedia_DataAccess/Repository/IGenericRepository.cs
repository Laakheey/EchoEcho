using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia_DataAccess.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetById(object id);
        Task Insert(T entity);
        void Update(T entity);
        Task Delete(object id);
        Task Save();
        IQueryable<T> GetAll();
        void DeleteAll(List<T> obj);
    }
}

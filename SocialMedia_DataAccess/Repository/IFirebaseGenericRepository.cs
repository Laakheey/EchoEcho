using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia_DataAccess.Repository
{
    public interface IFirebaseGenericRepository<T> where T : class
    {
        Task Delete(string id);
        Task DeleteAll(List<T> objs);
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(string id);
        Task Insert(T entity);
        Task Update(T entity);
    }
}

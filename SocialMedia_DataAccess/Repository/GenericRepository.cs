using Microsoft.EntityFrameworkCore;
using SocialMedia_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia_DataAccess.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        public SocialMediaDataContext _context;
        public DbSet<T> table = null;
        public GenericRepository(SocialMediaDataContext context)
        {
            this._context = context;
            table = _context.Set<T>();
        }
        public async Task Delete(object id)
        {
            T existing = await table.FindAsync(id);
            if (existing != null)
            {
                table.Remove(existing);
            }
        }

        public void DeleteAll(List<T> obj)
        {
            table.RemoveRange(obj);
        }

        public IQueryable<T> GetAll()
        {
            IQueryable<T> query = table;
            return query;
        }

        public async Task<T> GetById(object id)
        {
            return await table.FindAsync(id);
        }

        public async Task Insert(T entity)
        {
            await table.AddAsync(entity);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(T entity)
        {
            table.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}

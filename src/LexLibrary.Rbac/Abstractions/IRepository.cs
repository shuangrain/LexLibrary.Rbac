using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexLibrary.Rbac.Abstractions
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Query();

        void Add(T entity);

        void AddRange(IEnumerable<T> entities);

        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entities);

        IDbContextTransaction BeginTransaction();

        int SaveChanges();
    }
}

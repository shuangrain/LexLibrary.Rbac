using LexLibrary.Rbac.Abstractions;
using LexLibrary.Rbac.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LexLibrary.Rbac.Repositories
{
    public class LexLibraryRbacRepository<T> : IRepository<T>, IDisposable where T : class
    {
        protected readonly LexLibraryRbacDbContext Context = null;
        protected readonly DbSet<T> DbSet = null;

        public LexLibraryRbacRepository(LexLibraryRbacDbContext context)
        {
            Context = context;
            DbSet = Context.Set<T>();
        }

        public virtual void Add(T entity)
        {
            DbSet.Add(entity);
        }

        public virtual IDbContextTransaction BeginTransaction()
        {
            return Context.Database.BeginTransaction();
        }

        public virtual int SaveChanges()
        {
            validateEntities();
            return Context.SaveChanges();
        }

        public virtual IQueryable<T> Query()
        {
            return DbSet;
        }

        public virtual void Remove(T entity)
        {
            DbSet.Remove(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            DbSet.AddRange(entities);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            DbSet.RemoveRange(entities);
        }

        public virtual void Dispose()
        {
            Context.Dispose();
        }

        /// <summary>
        /// 進 DB 前檢查資料格式
        /// </summary>
        private void validateEntities()
        {
            var modifiedEntries = Context.ChangeTracker.Entries()
                    .Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in modifiedEntries)
            {
                if (entity.Entity is ValidatableObject validatableObject)
                {
                    var validationResults = validatableObject.Validate();
                    if (validationResults.Any())
                    {
                        throw new ValidationException(entity.Entity.GetType(), validationResults);
                    }
                }
            }
        }
    }
}

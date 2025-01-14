﻿using FilmManagement.Application.Abstracts.Repositories;
using FilmManagement.Domain.Entities;
using FilmManagement.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace FilmManagement.Persistence.Repositories
{
    public class EfBaseRepository<TEntity, TEntityId, TContext> : IBaseRepository<TEntity, TEntityId>
        where TEntity : BaseEntity<TEntityId>
        where TContext : DbContext
    {
        private readonly TContext _context;
        public EfBaseRepository(TContext context)
        {
            _context = context;
        }

        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            bool enableTracking = true,
            bool withDeleted = false
            )
        {
            IQueryable<TEntity> queryable = _context.Set<TEntity>();
            if (!enableTracking)
                queryable = queryable.AsNoTracking();
            if (withDeleted)
                queryable = queryable.Where(e => e.IsActive);
            if (include != null)
                queryable = include(queryable);
            return await queryable.FirstOrDefaultAsync(predicate);
        }

        public async Task<IList<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            bool enableTracking = true,
            bool withDeleted = false
            )
        {
            IQueryable<TEntity> queryable = _context.Set<TEntity>();
            if (!enableTracking)
                queryable = queryable.AsNoTracking();
            if (!withDeleted)
                queryable = queryable.Where(e => e.IsActive);
            else
                queryable = queryable.IgnoreQueryFilters(); 
            if (include != null)
                queryable = include(queryable);
            if (predicate != null)
                queryable = queryable.Where(predicate);
            return await queryable.ToListAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null, bool enableTracking = true, bool withDeleted = false)
        {
            IQueryable<TEntity> queryable = _context.Set<TEntity>();
            if (!enableTracking)
                queryable = queryable.AsNoTracking();
            if (withDeleted)
                queryable = queryable.Where(e => e.IsActive);
            if (predicate != null)
                queryable = queryable.Where(predicate);
            return await queryable.AnyAsync();
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<IList<TEntity>> AddRangeAsync(IList<TEntity> entities)
        {
            foreach (TEntity entity in entities)
                entity.CreatedDate = DateTime.UtcNow;

            await _context.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            return entities;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<IList<TEntity>> UpdateRangeAsync(IList<TEntity> entities)
        {
            foreach (TEntity entity in entities)
                entity.UpdatedDate = DateTime.UtcNow;

            _context.UpdateRange(entities);
            await _context.SaveChangesAsync();
            return entities;
        }

        public async Task<TEntity> DeleteAsync(TEntity entity, bool forceDelete = false)
        {
            if (forceDelete == false)
            {
                // Soft Delete
                entity.IsActive = false;
                entity.DeletedDate = DateTime.UtcNow;
                _context.Update(entity);
            }
            else
                // Hard Delete
                _context.Remove(entity);

            await _context.SaveChangesAsync();
            return null;
        }

        public async Task<IList<TEntity>> DeleteRangeAsync(IList<TEntity> entities, bool forceDelete = false)
        {
            if (forceDelete == false)
            {
                foreach (TEntity entity in entities)
                {
                    entity.IsActive = false;
                    entity.DeletedDate = DateTime.UtcNow;
                    _context.Update(entity);
                }
            }
            else
                _context.RemoveRange(entities);

            await _context.SaveChangesAsync();
            return null;
        }      
    }
}

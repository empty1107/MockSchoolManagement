using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MockSchoolManagement.Infrastructure.Repositories
{
    /// <summary>
    /// 默认仓储的通用功能实现，用于所有的领域模型
    /// </summary>
    /// <typeparam name="TEntity">当前传入仓储的实体类型</typeparam>
    /// <typeparam name="TPrimaryKey">传入仓储的主键类型</typeparam>
    public class RepositoryBase<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : class
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        protected readonly AppDbContext _dbContext;
        /// <summary>
        /// 通过泛型，从数据库上下文中获取领域模型，用于获取当前实体的信息
        /// </summary>
        public virtual DbSet<TEntity> Table => _dbContext.Set<TEntity>();

        public RepositoryBase(AppDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public IQueryable<TEntity> GetAll()
        {
            return Table.AsQueryable();
        }

        public List<TEntity> GetAllList()
        {
            return GetAll().ToList();
        }

        public async Task<List<TEntity>> GetAllListAsync()
        {
            return await GetAll().ToListAsync();
        }

        public List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).ToList();
        }

        public async Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await GetAll().Where(expression).ToListAsync();
        }

        public TEntity Single(Expression<Func<TEntity, bool>> expression)
        {
            return GetAll().Single(expression);
        }

        public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await GetAll().SingleAsync(expression);
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> expression)
        {
            return GetAll().FirstOrDefault(expression);
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression)
        {
            var entity = await GetAll().FirstOrDefaultAsync(expression);
            return entity;
        }

        public TEntity Insert(TEntity entity)
        {
            var newEntity = Table.Add(entity).Entity;
            Save();
            return newEntity;
        }

        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            var entityEntry = await Table.AddAsync(entity);
            await SaveAsync();
            return entityEntry.Entity;
        }

        public TEntity Update(TEntity entity)
        {
            AttachIfNot(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            Save();
            return entity;
        }
        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            AttachIfNot(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            await SaveAsync();
            return entity;
        }

        public void Delete(TEntity entity)
        {
            AttachIfNot(entity);
            Table.Remove(entity);
            Save();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            AttachIfNot(entity);
            Table.Remove(entity);
            await SaveAsync();
        }

        public void Delete(Expression<Func<TEntity, bool>> expression)
        {
            foreach (var entity in GetAll().Where(expression).ToList())
            {
                Delete(entity);
            }
        }

        public async Task DeleteAsync(Expression<Func<TEntity, bool>> expression)
        {
            foreach (var entity in GetAll().Where(expression).ToList())
            {
                await DeleteAsync(entity);
            }
        }

        public int Count()
        {
            return GetAll().Count();
        }

        public async Task<int> CountAsync()
        {
            return await GetAll().CountAsync();
        }

        public int Count(Expression<Func<TEntity, bool>> expression)
        {
            return GetAll().Where(expression).Count();
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await GetAll().Where(expression).CountAsync();
        }

        public long LongCount()
        {
            return GetAll().LongCount();
        }

        public async Task<long> LongCountAsync()
        {
            return await GetAll().LongCountAsync();
        }

        public long LongCount(Expression<Func<TEntity, bool>> expression)
        {
            return GetAll().Where(expression).LongCount();
        }

        public async Task<long> LongCountAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await GetAll().Where(expression).LongCountAsync();
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        protected async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 异步保存
        /// </summary>
        protected void Save()
        {
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// 检查实体是否处于跟踪状态，如果是，则返回；如果不是，则添加跟踪状态
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void AttachIfNot(TEntity entity)
        {
            var entry = _dbContext.ChangeTracker.Entries().FirstOrDefault(e => e.Entity == entity);
            if (entry != null)
            {
                return;
            }
            Table.Attach(entity);
        }
    }
}

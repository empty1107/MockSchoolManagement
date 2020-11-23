using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MockSchoolManagement.Infrastructure.Repositories
{
    /// <summary>
    /// 此接口是所有仓储的约定，此接口仅作为约定，用于标识它们
    /// </summary>
    /// <typeparam name="TEntity">当前传入仓储的实体类型</typeparam>
    /// <typeparam name="TPrimaryKey">传入仓储的主键类型</typeparam>
    public interface IRepository<TEntity, TPrimaryKey> where TEntity : class
    {
        #region 查询
        /// <summary>
        /// 用户从整个表中检索实体的IQueryable
        /// </summary>
        /// <returns>可用于从数据中选择实体</returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// 用户获取所有实体
        /// </summary>
        /// <returns>所有实体列表</returns>
        List<TEntity> GetAllList();

        /// <summary>
        /// 用户获取所有实体的异步实现
        /// </summary>
        /// <returns>所有实体列表</returns>
        Task<List<TEntity>> GetAllListAsync();

        /// <summary>
        /// 用于获取传入本方法的所有实体
        /// </summary>
        /// <param name="predicate">筛选实体的条件</param>
        /// <returns>所有实体列表</returns>
        List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 用于获取传入本方法的所有实体
        /// </summary>
        /// <param name="expression">筛选实体的条件</param>
        /// <returns>所有实体列表</returns>
        Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// 通过传入的筛选条件来获取实体信息，如果查询不到返回值，则会引发异常
        /// </summary>
        /// <param name="expression">Entity</param>
        /// <returns></returns>
        TEntity Single(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// 通过传入的筛选条件来获取实体信息，查询不到会引发异常
        /// </summary>
        /// <param name="expression">Entity</param>
        /// <returns></returns>
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// 通过传入的筛选条件来获取实体信息，查询不到返回null
        /// </summary>
        /// <param name="expression">筛选条件</param>
        /// <returns></returns>
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// 通过传入的筛选条件来获取实体信息，查询不到返回null
        /// </summary>
        /// <param name="expression">筛选条件</param>
        /// <returns></returns>
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression);

        #endregion

        #region Insert

        /// <summary>
        /// 添加一个新实体信息
        /// </summary>
        /// <param name="entity">被添加的实体</param>
        /// <returns></returns>
        TEntity Insert(TEntity entity);

        /// <summary>
        /// 添加一个新实体信息
        /// </summary>
        /// <param name="entity">被添加的实体</param>
        /// <returns></returns>
        Task<TEntity> InsertAsync(TEntity entity);

        #endregion

        #region Update
        /// <summary>
        /// 更新现有实体
        /// </summary>
        /// <param name="entity">被更新的实体</param>
        /// <returns></returns>
        TEntity Update(TEntity entity);

        /// <summary>
        /// 更新现有实体
        /// </summary>
        /// <param name="entity">被更新的实体</param>
        /// <returns></returns>
        Task<TEntity> UpdateAsync(TEntity entity);
        #endregion

        #region Delete
        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <param name="entity">被删除的实体</param>
        void Delete(TEntity entity);

        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <param name="entity">被删除的实体</param>
        Task DeleteAsync(TEntity entity);

        /// <summary>
        /// 按传入的条件可删除多个实体
        /// 注意：所有符合给定条件的实体都将被检索和删除
        /// 如果条件比较多，则待删除的实体也比较多，可能会导致主要的性能问题
        /// </summary>
        /// <param name="expression">条件</param>
        void Delete(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// 按传入的条件可删除多个实体
        /// 注意：所有符合给定条件的实体都将被检索和删除
        /// 如果条件比较多，则待删除的实体也比较多，可能会导致主要的性能问题
        /// </summary>
        /// <param name="expression">条件</param>
        Task DeleteAsync(Expression<Func<TEntity, bool>> expression);
        #endregion

        #region 总和计算

        /// <summary>
        /// 获取此仓储中所有实体的总和
        /// </summary>
        /// <returns>实体总和数</returns>
        int Count();

        /// <summary>
        /// 获取此仓储中所有实体的总和
        /// </summary>
        /// <returns>实体总和数</returns>
        Task<int> CountAsync();

        /// <summary>
        /// 计算仓储中的实体总和，可条件筛选
        /// </summary>
        /// <param name="expression">条件</param>
        /// <returns>实体总和数</returns>
        int Count(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// 计算仓储中的实体总和，可条件筛选
        /// </summary>
        /// <param name="expression">条件</param>
        /// <returns>实体总和数</returns>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// 获取此仓储中所有实体的总和（如果预期返回值大于Int.MaxValue，则推荐该方法）
        /// <see cref="int.MaxValue"/>
        /// </summary>
        /// <returns>实体总和数</returns>
        long LongCount();

        /// <summary>
        /// 获取此仓储中所有实体的总和（如果预期返回值大于Int.MaxValue，则推荐该方法）
        /// <see cref="int.MaxValue"/>
        /// </summary>
        /// <returns>实体总和数</returns>
        Task<long> LongCountAsync();

        /// <summary>
        /// 根据条件获取此仓储中所有实体的总和（如果预期返回值大于Int.MaxValue，则推荐该方法）
        /// <see cref="int.MaxValue"/>
        /// </summary>
        /// <param name="expression">条件</param>
        /// <returns>实体总和数</returns>
        long LongCount(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// 根据条件获取此仓储中所有实体的总和（如果预期返回值大于Int.MaxValue，则推荐该方法）
        /// <see cref="int.MaxValue"/>
        /// </summary>
        /// <param name="expression">条件</param>
        /// <returns>实体总和数</returns>
        Task<long> LongCountAsync(Expression<Func<TEntity, bool>> expression);
        #endregion
    }
}

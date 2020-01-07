using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace GTDFundAllocatorService.Repository.Shared.Contracts
{
    public interface IGeneralRepository<TEntity> : IGeneralRepository<TEntity, int>
        where TEntity : EntityBase<int>
    {
    }

    public interface IGeneralRepository<TEntity, TKey>
        where TEntity : EntityBase<TKey>
        where TKey : IEquatable<TKey>
    {
        Func<Queue<WithCondition<TEntity, TKey>>, Task<TEntity>> GetById(TKey id, bool needTracking = false);
        Task AddAsync(TEntity entity);
        Task AddMultipleAsync(IEnumerable<TEntity> entity);
        Task DeleteAsync(TEntity entity);
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);
        Task DeleteMultipleAsync(IEnumerable<TEntity> entity);
        Task EditAsync(TEntity entity, params string[] properties);
        Task EditAsync(TEntity entity, object concurrentStamp, params Expression<Func<TEntity, object>>[] properties);
        Task EditAsync(TEntity entity);
        Task EditNotTrackingAsync(TEntity entity);
        Task EditMultipleAsync(IEnumerable<TEntity> entities);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
        Func<Queue<WithCondition<TEntity, TKey>>, Func<SearchCriteria, Task<List<TEntity>>>> List(Expression<Func<TEntity, bool>> predicate = null, bool needTracking = false, bool sync = false);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default(CancellationToken));

        void Detached(TEntity entity);
    }

    public static class GeneralRepositoryExtensions
    {
        #region List Higher Order Function Extensions
        public static Func<Queue<WithCondition<TEntity, TKey>>, Func<SearchCriteria, Task<List<TEntity>>>> With<TEntity, TKey>(this Func<Queue<WithCondition<TEntity, TKey>>, Func<SearchCriteria, Task<List<TEntity>>>> highOrderObj, string relatedItem, bool needTracking = false)
            where TEntity : EntityBase<TKey>
             where TKey : IEquatable<TKey>
        {
            return relatedItems =>
            {
                relatedItems.Enqueue(new WithCondition<TEntity, TKey>(relatedItem) { NeedTracking = needTracking });

                return highOrderObj(relatedItems);
            };
        }

        public static Func<Queue<WithCondition<TEntity, TKey>>, Func<SearchCriteria, Task<List<TEntity>>>> Then<TEntity, TKey>(this Func<Queue<WithCondition<TEntity, TKey>>, Func<SearchCriteria, Task<List<TEntity>>>> highOrderObj, string relatedItem, bool needTracking = false)
         where TEntity : EntityBase<TKey>
          where TKey : IEquatable<TKey>
        {
            return relatedItems =>
            {
                relatedItems.Enqueue(new WithCondition<TEntity, TKey>(relatedItem) { NeedCombine = true, NeedTracking = needTracking });
                return highOrderObj(relatedItems);
            };
        }

        public static Func<Queue<WithCondition<TEntity, TKey>>, Func<SearchCriteria, Task<List<TEntity>>>> Skip<TEntity, TKey>(this Func<Queue<WithCondition<TEntity, TKey>>, Func<SearchCriteria, Task<List<TEntity>>>> highOrderObj, int skip)
          where TEntity : EntityBase<TKey>
             where TKey : IEquatable<TKey>
        {
            return relatedItems =>
            {
                return searchCriteria =>
                {
                    searchCriteria.Skip = skip;

                    return highOrderObj(relatedItems)(searchCriteria);
                };
            };
        }

        public static Func<Queue<WithCondition<TEntity, TKey>>, Func<SearchCriteria, Task<List<TEntity>>>> Take<TEntity, TKey>(this Func<Queue<WithCondition<TEntity, TKey>>, Func<SearchCriteria, Task<List<TEntity>>>> highOrderObj, int take)
            where TEntity : EntityBase<TKey>
             where TKey : IEquatable<TKey>
        {
            return relatedItems =>
            {
                return searchCriteria =>
                {
                    searchCriteria.Take = take;

                    return highOrderObj(relatedItems)(searchCriteria);
                };
            };
        }

        public static Func<Queue<WithCondition<TEntity, TKey>>, Func<SearchCriteria, Task<List<TEntity>>>> OrderByDescending<TEntity, TKey>(this Func<Queue<WithCondition<TEntity, TKey>>, Func<SearchCriteria, Task<List<TEntity>>>> highOrderObj, string sortBy)
          where TEntity : EntityBase<TKey>
             where TKey : IEquatable<TKey>
        {
            return relatedItems =>
            {
                return searchCriteria =>
                {
                    searchCriteria.SortBy = sortBy;

                    searchCriteria.IsDescending = true;

                    return highOrderObj(relatedItems)(searchCriteria);
                };
            };
        }

        public static Func<Queue<WithCondition<TEntity, TKey>>, Func<SearchCriteria, Task<List<TEntity>>>> OrderBy<TEntity, TKey>(this Func<Queue<WithCondition<TEntity, TKey>>, Func<SearchCriteria, Task<List<TEntity>>>> highOrderObj, string sortBy)
         where TEntity : EntityBase<TKey>
            where TKey : IEquatable<TKey>
        {
            return relatedItems =>
            {
                return searchCriteria =>
                {
                    searchCriteria.SortBy = sortBy;

                    searchCriteria.IsDescending = false;

                    return highOrderObj(relatedItems)(searchCriteria);
                };
            };
        }

        public static Func<Queue<WithCondition<TEntity, TKey>>, Func<SearchCriteria, Task<List<TEntity>>>> ThenSort<TEntity, TKey>(this Func<Queue<WithCondition<TEntity, TKey>>, Func<SearchCriteria, Task<List<TEntity>>>> highOrderObj, string thenBy, bool isDescending)
         where TEntity : EntityBase<TKey>
            where TKey : IEquatable<TKey>
        {
            return relatedItems =>
            {
                return searchCriteria =>
                {
                    searchCriteria.ThenBy.Add(Tuple.Create(thenBy, isDescending));

                    return highOrderObj(relatedItems)(searchCriteria);
                };
            };
        }

        public static Func<Queue<WithCondition<TEntity, TKey>>, Func<SearchCriteria, Task<List<TEntity>>>> ThenBy<TEntity, TKey>(this Func<Queue<WithCondition<TEntity, TKey>>, Func<SearchCriteria, Task<List<TEntity>>>> highOrderObj, string thenBy)
         where TEntity : EntityBase<TKey>
            where TKey : IEquatable<TKey>
        {
            return relatedItems =>
            {
                return searchCriteria =>
                {
                    searchCriteria.ThenBy.Add(Tuple.Create(thenBy, false));

                    return highOrderObj(relatedItems)(searchCriteria);
                };
            };
        }

        public static Func<Queue<WithCondition<TEntity, TKey>>, Func<SearchCriteria, Task<List<TEntity>>>> ThenByDescending<TEntity, TKey>(this Func<Queue<WithCondition<TEntity, TKey>>, Func<SearchCriteria, Task<List<TEntity>>>> highOrderObj, string thenBy)
         where TEntity : EntityBase<TKey>
            where TKey : IEquatable<TKey>
        {
            return relatedItems =>
            {
                return searchCriteria =>
                {
                    searchCriteria.ThenBy.Add(Tuple.Create(thenBy, true));

                    return highOrderObj(relatedItems)(searchCriteria);
                };
            };
        }

        public static async Task<int> Count<TEntity, TKey>(this Func<Queue<WithCondition<TEntity, TKey>>, Func<SearchCriteria, Task<List<TEntity>>>> highOrderObj)
         where TEntity : EntityBase<TKey>
             where TKey : IEquatable<TKey>
        {
            return (await highOrderObj(new Queue<WithCondition<TEntity, TKey>>())(new SearchCriteria())).Count;
        }

        public static Task<List<TEntity>> Result<TEntity, TKey>(this Func<Queue<WithCondition<TEntity, TKey>>, Func<SearchCriteria, Task<List<TEntity>>>> highOrderObj)
            where TEntity : EntityBase<TKey>
             where TKey : IEquatable<TKey>
        {
            return highOrderObj(new Queue<WithCondition<TEntity, TKey>>())(new SearchCriteria());
        }
        #endregion

        #region GetById Higher Order Function Extensions
        public static Func<Queue<WithCondition<TEntity, TKey>>, Task<TEntity>> With<TEntity, TKey>(this Func<Queue<WithCondition<TEntity, TKey>>, Task<TEntity>> highOrderObj, string relatedItem, bool needTracking = false)
            where TEntity : EntityBase<TKey>
             where TKey : IEquatable<TKey>
        {
            return relatedItems =>
            {
                relatedItems.Enqueue(new WithCondition<TEntity, TKey>(relatedItem) { NeedTracking = needTracking });

                return highOrderObj(relatedItems);
            };
        }
        public static Func<Queue<WithCondition<TEntity, TKey>>, Task<TEntity>> Then<TEntity, TKey>(this Func<Queue<WithCondition<TEntity, TKey>>, Task<TEntity>> highOrderObj, string relatedItem, bool needTracking = false)
            where TEntity : EntityBase<TKey>
             where TKey : IEquatable<TKey>
        {
            return relatedItems =>
            {
                relatedItems.Enqueue(new WithCondition<TEntity, TKey>(relatedItem) { NeedCombine = true, NeedTracking = needTracking });

                return highOrderObj(relatedItems);
            };
        }
        public static Task<TEntity> Result<TEntity, TKey>(this Func<Queue<WithCondition<TEntity, TKey>>, Task<TEntity>> highOrderObj)
             where TEntity : EntityBase<TKey>
             where TKey : IEquatable<TKey>
        {
            return highOrderObj(new Queue<WithCondition<TEntity, TKey>>());
        }
        #endregion
    }
}

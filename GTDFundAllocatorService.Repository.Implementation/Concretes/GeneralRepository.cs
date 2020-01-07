using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GTDFundAllocatorService.Foundation.Common;
using GTDFundAllocatorService.Repository.Shared;
using GTDFundAllocatorService.Repository.Shared.Contracts;
using Microsoft.EntityFrameworkCore;

namespace GTDFundAllocatorService.Repository.Implementation
{
    public class GeneralRepository<TEntity> : GeneralRepository<TEntity, int>, IGeneralRepository<TEntity>
        where TEntity : EntityBase<int>
    {
        public GeneralRepository(FundAllocatorDbContext dbContext) : base(dbContext)
        {

        }
    }

    public class GeneralRepository<TEntity, TKey> : IGeneralRepository<TEntity, TKey>
        where TEntity : EntityBase<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly FundAllocatorDbContext _dbContext;

        public GeneralRepository(FundAllocatorDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task AddAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Add(entity);
            return _dbContext.SaveChangesAsync();
        }

        public Task AddMultipleAsync(IEnumerable<TEntity> entities)
        {
            if (!entities.Any())
            {
                return Task.FromResult(0);
            }

            _dbContext.Set<TEntity>().AddRange(entities);
            return _dbContext.SaveChangesAsync();
        }

        public Task DeleteAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
            return _dbContext.SaveChangesAsync();
        }

        public Task DeleteMultipleAsync(IEnumerable<TEntity> entity)
        {
            _dbContext.Set<TEntity>().RemoveRange(entity);
            return _dbContext.SaveChangesAsync();
        }

        public Task EditAsync(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            return _dbContext.SaveChangesAsync();
        }

        public Task EditNotTrackingAsync(TEntity entity)
        {
            _dbContext.Update(entity);
            _dbContext.SaveChanges();

            _dbContext.Entry(entity).State = EntityState.Detached;

            return Task.FromResult(1);
        }

        public Task EditAsync(TEntity entity, params string[] properties)
        {
            _dbContext.Set<TEntity>().Attach(entity);
            foreach (var prop in properties)
            {
                _dbContext.Entry(entity).Property(prop).IsModified = true;
            }

            return _dbContext.SaveChangesAsync();
        }

        public Task EditAsync(TEntity entity, object concurrentStamp, params Expression<Func<TEntity, object>>[] properties)
        {
            _dbContext.Entry(entity).Property("ConcurrentStamp").OriginalValue = concurrentStamp;

            foreach (var prop in properties)
            {
                _dbContext.Entry(entity).Property(prop).IsModified = true;
            }

            return _dbContext.SaveChangesAsync();
        }

        public Task EditMultipleAsync(IEnumerable<TEntity> entities)
        {
            if (!entities.Any())
            {
                return Task.FromResult(0);
            }

            _dbContext.UpdateRange(entities);
            return _dbContext.SaveChangesAsync();
        }

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbContext.Set<TEntity>().AnyAsync(predicate);
        }

        public Func<Queue<WithCondition<TEntity, TKey>>, Task<TEntity>> GetById(TKey id, bool needTracking = false)
        {
            return relatedItems =>
            {
                var query = needTracking ? _dbContext.Set<TEntity>().AsQueryable() : _dbContext.Set<TEntity>().AsQueryable().AsNoTracking();

                return query.Extend(relatedItems).SingleOrDefaultAsync(q => q.Id.Equals(id));
            };
        }

        public Func<Queue<WithCondition<TEntity, TKey>>, Func<SearchCriteria, Task<List<TEntity>>>> List(Expression<Func<TEntity, bool>> predicate = null, bool needTracking = false, bool sync = false)
        {
            return relatedItems =>
            {
                return searchCriteria =>
                {
                    bool needPagination = searchCriteria.Skip.HasValue && searchCriteria.Take.HasValue;

                    if (needPagination && searchCriteria.Take.Value == 0)
                    {
                        return Task.FromResult(new List<TEntity>());
                    }

                    bool needSorting = !string.IsNullOrWhiteSpace(searchCriteria.SortBy);

                    Expression<Func<TEntity, bool>> conditions = predicate != null ? PredicateBuilder.Create(predicate) : PredicateBuilder.Create<TEntity>(p => true);

                    IQueryable<TEntity> query = needTracking ? _dbContext.Set<TEntity>().AsQueryable() : _dbContext.Set<TEntity>().AsQueryable().AsNoTracking();

                    var queryWhere = query.Extend(relatedItems).Where(conditions);

                    if (needSorting)
                    {
                        var querySort = searchCriteria.IsDescending ?
                            queryWhere.OrderByDescending(searchCriteria.SortBy) :
                            queryWhere.OrderBy(searchCriteria.SortBy);

                        queryWhere = querySort.Sort<TEntity, TKey>(searchCriteria);
                    }

                    if (needPagination)
                    {
                        queryWhere = queryWhere
                           .Skip(searchCriteria.Skip.Value)
                           .Take(searchCriteria.Take.Value);
                    }

                    if (sync)
                    {
                        return Task.FromResult(queryWhere.ToList());
                    }

                    return queryWhere.ToListAsync();
                };
            };
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>().AsNoTracking().AsQueryable();
            if (predicate != null)
            {
                query = query.Where(PredicateBuilder.Create(predicate));
            }
            return query.CountAsync(cancellationToken);
        }

        public Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var entities = _dbContext.Set<TEntity>().AsNoTracking().Where(predicate);

            if (entities.Count() > 0)
            {
                _dbContext.RemoveRange(entities);

                return _dbContext.SaveChangesAsync();
            }

            return Task.FromResult(1);
        }

        public void Detached(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Detached;
        }
    }

    public static class GeneralRepositoryExtensions
    {
        public static IQueryable<TEntity> Extend<TEntity, TKey>(this IQueryable<TEntity> query, Queue<WithCondition<TEntity, TKey>> conditions)
                where TEntity : EntityBase<TKey>
            where TKey : IEquatable<TKey>
        {
            var then = new StringBuilder();
            while (conditions.TryDequeue(out WithCondition<TEntity, TKey> item))
            {
                if (item.NeedCombine)
                {
                    if (string.IsNullOrEmpty(then.ToString()))
                        then.Append(item.NavigationPropertyPath);
                    else
                        then.Insert(0, $"{item.NavigationPropertyPath}.");

                    continue;
                }

                query = string.IsNullOrEmpty(then.ToString())
                        ? query.Include(item.NavigationPropertyPath)
                        : query.Include($"{item.NavigationPropertyPath}.{then}");

                if (!item.NeedTracking)
                {
                    query = query.AsNoTracking();
                }

                then.Clear();
            }

            return query;
        }

        public static IOrderedQueryable<TEntity> Sort<TEntity, TKey>(this IOrderedQueryable<TEntity> query, SearchCriteria searchCriteria)
                where TEntity : EntityBase<TKey>
            where TKey : IEquatable<TKey>
        {
            if (searchCriteria.ThenBy.Any())
            {
                foreach (var item in searchCriteria.ThenBy.Reverse())
                {
                    query = item.Item2 ?
                        query.ThenByDescending(item.Item1) :
                        query.ThenBy(item.Item1);
                }
            }

            return query;
        }

        private static IOrderedQueryable<TEntity> BuildSortQuery<TEntity>(this IQueryable<TEntity> source, string memberPath, string method)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "item");

            var member = memberPath.Split('.')
                .Aggregate((Expression)parameter, Expression.PropertyOrField);

            var keySelector = Expression.Lambda(member, parameter);

            var methodCall = Expression.Call(
                typeof(Queryable), method, new[] { parameter.Type, member.Type },
                source.Expression, Expression.Quote(keySelector));

            return (IOrderedQueryable<TEntity>)source.Provider.CreateQuery(methodCall);
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string memberPath)
        {
            return source.BuildSortQuery(memberPath, "OrderBy");
        }
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string memberPath)
        {
            return source.BuildSortQuery(memberPath, "OrderByDescending");
        }
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string memberPath)
        {
            return source.BuildSortQuery(memberPath, "ThenBy");
        }
        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string memberPath)
        {
            return source.BuildSortQuery(memberPath, "ThenByDescending");
        }
    }
}

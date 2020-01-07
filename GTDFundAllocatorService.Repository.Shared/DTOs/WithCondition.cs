using System;
using System.Linq.Expressions;

namespace GTDFundAllocatorService.Repository.Shared
{
    public enum RelationalOperator
    {
        AND,
        OR
    }

    public class WithCondition<TEntity, TKey>
    {
        public WithCondition(string navigationPropertyPath)
            : this(navigationPropertyPath, RelationalOperator.AND)
        {
        }

        public WithCondition(string navigationPropertyPath, RelationalOperator operatorTag)
            : this(navigationPropertyPath, operatorTag, p => true)
        {
        }

        public WithCondition(string navigationPropertyPath, RelationalOperator operatorTag, Expression<Func<TEntity, bool>> predicate)
        {
            NavigationPropertyPath = navigationPropertyPath;

            RelationalOperator = operatorTag;

            Predicate = predicate;
        }

        public string NavigationPropertyPath { get; set; }
        public RelationalOperator RelationalOperator { get; set; }
        public Expression<Func<TEntity, bool>> Predicate { get; set; }
        public bool NeedCombine { get; set; }
        public bool NeedTracking { get; set; }

        public void Deconstruct(out string navigationPropertyPath, out RelationalOperator operatorTag, out Expression<Func<TEntity, bool>> predicate, out bool needTracking)
        {
            navigationPropertyPath = NavigationPropertyPath;

            operatorTag = RelationalOperator;

            predicate = Predicate;

            needTracking = NeedTracking;
        }
    }
}

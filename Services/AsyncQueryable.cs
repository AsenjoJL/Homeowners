using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOMEOWNER.Services
{
    /// <summary>
    /// Wrapper to provide IQueryable-like interface for Firebase async collections
    /// </summary>
    public class AsyncQueryable<T> : IQueryable<T>
    {
        private readonly Task<List<T>> _dataTask;
        private List<T>? _cachedData;

        public AsyncQueryable(Task<List<T>> dataTask)
        {
            _dataTask = dataTask;
        }

        public Type ElementType => typeof(T);
        public System.Linq.Expressions.Expression Expression => System.Linq.Expressions.Expression.Constant(this);
        public IQueryProvider Provider => new AsyncQueryProvider<T>(this);

        public IEnumerator<T> GetEnumerator()
        {
            if (_cachedData == null)
            {
                _cachedData = _dataTask.GetAwaiter().GetResult();
            }
            return _cachedData.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class AsyncQueryProvider<T> : IQueryProvider
    {
        private readonly AsyncQueryable<T> _queryable;

        public AsyncQueryProvider(AsyncQueryable<T> queryable)
        {
            _queryable = queryable;
        }

        public IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
        {
            return _queryable;
        }

        public IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression)
        {
            return (IQueryable<TElement>)_queryable;
        }

        public object Execute(System.Linq.Expressions.Expression expression)
        {
            return _queryable.ToList();
        }

        public TResult Execute<TResult>(System.Linq.Expressions.Expression expression)
        {
            return (TResult)(object)_queryable.ToList();
        }
    }
}


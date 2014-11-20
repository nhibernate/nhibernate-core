using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using NHibernate.Engine;
using NHibernate.Transform;
using Remotion.Linq;

namespace NHibernate.Linq
{
    /// <summary>
    /// Provides the main entry point to a LINQ query.
    /// </summary>
    public class NhQueryable<T> : QueryableBase<T>, IQueryOptions
    {
        // This constructor is called by our users, create a new IQueryExecutor.
        public NhQueryable(ISessionImplementor session)
            : base(new DefaultQueryProvider(session))
        {
        }

        // This constructor is called indirectly by LINQ's query methods, just pass to base.
        public NhQueryable(IQueryProvider provider, Expression expression)
            : base(provider, expression)
        {
        }

		#region IQueryOptions Members

	    public IList List()
	    {
		    return this.ToList();
	    }

	    public void List(IList list)
	    {
		    foreach (var entity in this)
		    {
			    list.Add(entity);
		    }
	    }

		IList<TEntity> IQueryOptions.List<TEntity>()
	    {
		    return this.ToList().Cast<TEntity>().ToList();
	    }

	    public object UniqueResult()
	    {
		    return this.SingleOrDefault();
	    }

		TEntity IQueryOptions.UniqueResult<TEntity>()
		{
			return (TEntity) (object) this.SingleOrDefault();
		}

		public IQueryOptions SetFirstResult(int firstResult)
		{
			return this.Skip(firstResult) as IQueryOptions;
		}

		public IQueryOptions SetReadOnly(bool readOnly)
		{
			//NH-3470
			return LinqExtensionMethods.ReadOnly(this, readOnly) as IQueryOptions;
		}

		public IQueryOptions SetCacheable(bool cacheable)
		{
			return LinqExtensionMethods.Cacheable(this) as IQueryOptions;
		}

		public IQueryOptions SetCacheRegion(string cacheRegion)
		{
			return LinqExtensionMethods.CacheRegion(this, cacheRegion) as IQueryOptions;
		}

		public IQueryOptions SetTimeout(int timeout)
		{
			return LinqExtensionMethods.Timeout(this, timeout) as IQueryOptions;
		}

		public IQueryOptions SetMaxResults(int maxResults)
		{
			return this.Take(maxResults) as IQueryOptions;
		}

		public IQueryOptions SetFetchSize(int maxResults)
		{
			return LinqExtensionMethods.FetchSize(this, maxResults) as IQueryOptions;
		}

		public IQueryOptions SetLockMode(string alias, LockMode lockMode)
		{
			//NH-2285
			return LinqExtensionMethods.LockMode(this, lockMode) as IQueryOptions;
		}

		public IQueryOptions SetResultTransformer(IResultTransformer resultTransformer)
		{
			//NH-3299
			return LinqExtensionMethods.ResultTransformer(this, resultTransformer) as IQueryOptions;
		}

		IFutureValue<TEntity> IQueryOptions.FutureValue<TEntity>()
	    {
		    return LinqExtensionMethods.ToFutureValue(this) as IFutureValue<TEntity>;
	    }

		IEnumerable<TEntity> IQueryOptions.Future<TEntity>()
	    {
			return LinqExtensionMethods.ToFuture(this).Cast<TEntity>();
	    }

		#endregion
	}
}
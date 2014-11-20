using System.Collections;
using System.Collections.Generic;
using NHibernate.Transform;

namespace NHibernate
{
	public interface IQueryOptions
	{
		IList List();

		void List(IList list);

		IList<T> List<T>();

		object UniqueResult();

		T UniqueResult<T>();

		IQueryOptions SetFirstResult(int firstResult);

		IQueryOptions SetFetchSize(int fetchSize);

		IQueryOptions SetReadOnly(bool readOnly);

		IQueryOptions SetCacheable(bool cacheable);

		IQueryOptions SetCacheRegion(string cacheRegion);

		IQueryOptions SetTimeout(int timeout);

		IQueryOptions SetMaxResults(int maxResults);

		IQueryOptions SetLockMode(string alias, LockMode lockMode);

		IQueryOptions SetResultTransformer(IResultTransformer resultTransformer);

		IFutureValue<T> FutureValue<T>();

		IEnumerable<T> Future<T>();
	}
}

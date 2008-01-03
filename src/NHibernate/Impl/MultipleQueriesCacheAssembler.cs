using System.Collections;
using Iesi.Collections;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Impl
{
	internal class MultipleQueriesCacheAssembler : ICacheAssembler
	{
		private IList assemblersList;

		public MultipleQueriesCacheAssembler(IList assemblers)
		{
			assemblersList = assemblers;
		}

		public object Disassemble(object value, ISessionImplementor session, object owner)
		{
			IList srcList = (IList)value;
			ArrayList cacheable = new ArrayList();
			for (int i = 0; i < srcList.Count; i++)
			{
				ICacheAssembler[] assemblers = (ICacheAssembler[])assemblersList[i];
				IList itemList = (IList)srcList[i];
				ArrayList singleQueryCached = new ArrayList();
				foreach (object objToCache in itemList)
				{
					if (assemblers.Length == 1)
					{
						singleQueryCached.Add(assemblers[0].Disassemble(objToCache, session, owner));
					}
					else
					{
						singleQueryCached.Add(TypeFactory.Disassemble((object[]) objToCache, assemblers, null, session, null));
					}
				}
				cacheable.Add(singleQueryCached);
			}
			return cacheable;
		}

		public object Assemble(object cached, ISessionImplementor session, object owner)
		{
			IList srcList = (IList)cached;
			ArrayList result = new ArrayList();
			for (int i = 0; i < assemblersList.Count; i++)
			{
				ICacheAssembler[] assemblers = (ICacheAssembler[])assemblersList[i];
				IList queryFromCache = (IList)srcList[i];
				ArrayList queryResults = new ArrayList();
				foreach (object fromCache in queryFromCache)
				{
					if (assemblers.Length == 1)
					{
						queryResults.Add(assemblers[0].Assemble(fromCache, session, owner));
					}
					else
					{
						queryResults.Add(TypeFactory.Assemble((object[])fromCache, assemblers, session, owner));
					}
				}
				result.Add(queryResults);
			}
			return result;
		}

		public void BeforeAssemble(object cached, ISessionImplementor session)
		{
		}

		public IList GetResultFromQueryCache(
			ISessionImplementor session,
			QueryParameters queryParameters,
			ISet querySpaces,
			IQueryCache queryCache,
			QueryKey key)
		{
			if (!queryParameters.ForceCacheRefresh)
			{
				IList list = queryCache.Get(key, new ICacheAssembler[] { this }, querySpaces, session);
				//we had to wrap the query results in another list in order to save all
				//the queries in the same bucket, now we need to do it the other way around.
				if (list != null)
					list = (IList)list[0];
				return list;
			}
			return null;
		}

	}
}
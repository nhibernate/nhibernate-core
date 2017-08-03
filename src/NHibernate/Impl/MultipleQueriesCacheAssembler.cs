using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Impl
{
	internal class MultipleQueriesCacheAssembler : ICacheAssembler
	{
		private readonly IList assemblersList;

		public MultipleQueriesCacheAssembler(IList assemblers)
		{
			assemblersList = assemblers;
		}

		#region ICacheAssembler Members

		public object Disassemble(object value, ISessionImplementor session, object owner)
		{
			IList srcList = (IList) value;
			var cacheable = new List<object>();
			for (int i = 0; i < srcList.Count; i++)
			{
				ICacheAssembler[] assemblers = (ICacheAssembler[]) assemblersList[i];
				IList itemList = (IList) srcList[i];
				var singleQueryCached = new List<object>();
				foreach (object objToCache in itemList)
				{
					if (objToCache != null)
					{
						var valuesToCache = objToCache as object[];
						var assemblersToCache = assemblers;

						if (valuesToCache != null)
						{
							assemblersToCache = assemblers.Where((x, index) => valuesToCache[index] != null).ToArray();
							valuesToCache = valuesToCache.Where(x => x != null).ToArray();
						}
						else
						{
							valuesToCache = new object[] { objToCache };
						}

						if (valuesToCache.Length == 1)
						{
							singleQueryCached.Add(assemblers[0].Disassemble(valuesToCache[0], session, owner));
						}
						else
						{
							singleQueryCached.Add(TypeHelper.Disassemble(valuesToCache, assemblersToCache, null, session, null));
						}
					}
				}
				cacheable.Add(singleQueryCached);
			}
			return cacheable;
		}

		public object Assemble(object cached, ISessionImplementor session, object owner)
		{
			IList srcList = (IList) cached;
			var result = new List<object>();
			for (int i = 0; i < assemblersList.Count; i++)
			{
				ICacheAssembler[] assemblers = (ICacheAssembler[]) assemblersList[i];
				IList queryFromCache = (IList) srcList[i];
				var queryResults = new List<object>();
				foreach (object fromCache in queryFromCache)
				{
					if (assemblers.Length == 1)
					{
						queryResults.Add(assemblers[0].Assemble(fromCache, session, owner));
					}
					else
					{
						queryResults.Add(TypeHelper.Assemble((object[]) fromCache, assemblers, session, owner));
					}
				}
				result.Add(queryResults);
			}
			return result;
		}

		public void BeforeAssemble(object cached, ISessionImplementor session) {}

		#endregion

		public IList GetResultFromQueryCache(ISessionImplementor session, QueryParameters queryParameters,
											 ISet<string> querySpaces, IQueryCache queryCache, QueryKey key)
		{
			if (!queryParameters.ForceCacheRefresh)
			{
				IList list =
					queryCache.Get(key, new ICacheAssembler[] {this}, queryParameters.NaturalKeyLookup, querySpaces, session);
				//we had to wrap the query results in another list in order to save all
				//the queries in the same bucket, now we need to do it the other way around.
				if (list != null)
				{
					list = (IList) list[0];
				}
				return list;
			}
			return null;
		}
	}
}
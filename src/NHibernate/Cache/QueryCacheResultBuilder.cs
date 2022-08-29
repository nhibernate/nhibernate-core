using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Collection;
using NHibernate.Type;

namespace NHibernate.Cache
{
	/// <summary>
	/// A builder that builds a list from a query that can be passed to <see cref="IBatchableQueryCache"/>.
	/// </summary>
	public sealed class QueryCacheResultBuilder
	{
		private readonly IType[] _resultTypes;
		private readonly Loader.Loader.QueryCacheInfo _cacheInfo;

		public static bool IsCacheWithFetches(Loader.Loader loader)
		{
			return loader.CacheTypes.Length > loader.ResultTypes.Length;
		}

		internal QueryCacheResultBuilder(Loader.Loader loader)
		{
			_resultTypes = loader.ResultTypes;

			if (IsCacheWithFetches(loader))
			{
				_cacheInfo = loader.CacheInfo;
			}
		}

		internal IList Result { get; } = new List<object>();

		internal void AddRow(object result, object[] entities, IPersistentCollection[] collections)
		{
			if (_cacheInfo == null)
			{
				Result.Add(result);
				return;
			}

			var row = new object[_cacheInfo.CacheTypes.Length];
			if (_resultTypes.Length == 1)
			{
				row[0] = result;
			}
			else
			{
				Array.Copy((object[]) result, 0, row, 0, _resultTypes.Length);
			}

			var i = _resultTypes.Length;
			foreach (var index in _cacheInfo.AdditionalEntities)
			{
				row[i++] = entities[index];
			}

			if (collections != null)
			{
				foreach (var collection in collections)
				{
					row[i++] = collection;
				}
			}

			Result.Add(row);
		}

		internal IList GetResultList(IList cacheList)
		{
			if (_cacheInfo == null)
			{
				return cacheList;
			}

			var result = new List<object>();

			foreach (object[] cacheRow in cacheList)
			{
				if (_resultTypes.Length == 1)
				{
					result.Add(cacheRow[0]);
				}
				else
				{
					var row = new object[_resultTypes.Length];
					Array.Copy(cacheRow, 0, row, 0, _resultTypes.Length);
					result.Add(row);
				}
			}

			return result;
		}
	}
}

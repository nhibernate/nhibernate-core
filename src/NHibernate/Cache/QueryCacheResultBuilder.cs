using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Type;

namespace NHibernate.Cache
{
	/// <summary>
	/// A builder that builds a list from a query that can be passed to <see cref="IBatchableQueryCache"/>.
	/// </summary>
	public sealed class QueryCacheResultBuilder
	{
		private readonly IType[] _resultTypes;
		private readonly IType[] _cacheTypes;
		private readonly List<int> _entityFetchIndexes = new List<int>();
		private readonly List<int> _collectionFetchIndexes = new List<int>();
		private readonly bool _hasFetches;

		internal QueryCacheResultBuilder(Loader.Loader loader)
		{
			_resultTypes = loader.ResultTypes;
			_cacheTypes = loader.CacheTypes;

			if (loader.EntityFetches == null)
			{
				return;
			}

			for (var i = 0; i < loader.EntityFetches.Length; i++)
			{
				if (loader.EntityFetches[i])
				{
					_entityFetchIndexes.Add(i);
				}
			}

			_hasFetches = _entityFetchIndexes.Count > 0;
			if (loader.CollectionFetches == null)
			{
				return;
			}

			for (var i = 0; i < loader.CollectionFetches.Length; i++)
			{
				if (loader.CollectionFetches[i])
				{
					_collectionFetchIndexes.Add(i);
				}
			}
		}

		internal IList Result { get; } = new List<object>();

		internal void AddRow(object result, object[] entities, IPersistentCollection[] collections)
		{
			if (!_hasFetches)
			{
				Result.Add(result);
				return;
			}

			var row = new object[_cacheTypes.Length];
			if (_resultTypes.Length == 1)
			{
				row[0] = result;
			}
			else
			{
				Array.Copy((object[]) result, 0, row, 0, _resultTypes.Length);
			}

			var i = _resultTypes.Length;
			foreach (var index in _entityFetchIndexes)
			{
				row[i++] = entities[index];
			}

			foreach (var index in _collectionFetchIndexes)
			{
				row[i++] = collections[index];
			}

			Result.Add(row);
		}

		internal IList GetResultList(IList cacheList)
		{
			if (!_hasFetches)
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

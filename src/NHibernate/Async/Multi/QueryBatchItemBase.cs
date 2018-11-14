﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Multi
{
	using System.Threading.Tasks;
	using System.Threading;
	public abstract partial class QueryBatchItemBase<TResult> : IQueryBatchItem<TResult>
	{

		/// <inheritdoc />
		public async Task<int> ProcessResultsSetAsync(DbDataReader reader, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfNotInitialized();

			var dialect = Session.Factory.Dialect;
			var hydratedObjects = new List<object>[_queryInfos.Count];

			using (Session.SwitchCacheMode(_cacheMode))
			{
				var rowCount = 0;
				for (var i = 0; i < _queryInfos.Count; i++)
				{
					var queryInfo = _queryInfos[i];
					var loader = queryInfo.Loader;
					var queryParameters = queryInfo.Parameters;

					//Skip processing for items already loaded from cache
					if (queryInfo.IsResultFromCache)
					{
						continue;
					}

					var entitySpan = loader.EntityPersisters.Length;
					hydratedObjects[i] = entitySpan == 0 ? null : new List<object>(entitySpan);
					var keys = new EntityKey[entitySpan];

					var selection = queryParameters.RowSelection;
					var createSubselects = loader.IsSubselectLoadingEnabled;

					_subselectResultKeys[i] = createSubselects ? new List<EntityKey[]>() : null;
					var maxRows = Loader.Loader.HasMaxRows(selection) ? selection.MaxRows : int.MaxValue;
					var advanceSelection = !dialect.SupportsLimitOffset || !loader.UseLimit(selection, dialect);

					if (advanceSelection)
					{
						await (Loader.Loader.AdvanceAsync(reader, selection, cancellationToken)).ConfigureAwait(false);
					}

					var forcedResultTransformer = queryInfo.CacheKey?.ResultTransformer;
					if (queryParameters.HasAutoDiscoverScalarTypes)
					{
						loader.AutoDiscoverTypes(reader, queryParameters, forcedResultTransformer);
					}

					var lockModeArray = loader.GetLockModes(queryParameters.LockModes);
					var optionalObjectKey = Loader.Loader.GetOptionalObjectKey(queryParameters, Session);
					var tmpResults = new List<object>();

					for (var count = 0; count < maxRows && await (reader.ReadAsync(cancellationToken)).ConfigureAwait(false); count++)
					{
						rowCount++;

						var o =
							await (loader.GetRowFromResultSetAsync(
								reader,
								Session,
								queryParameters,
								lockModeArray,
								optionalObjectKey,
								hydratedObjects[i],
								keys,
								true,
								forcedResultTransformer
, cancellationToken							)).ConfigureAwait(false);
						if (loader.IsSubselectLoadingEnabled)
						{
							_subselectResultKeys[i].Add(keys);
							keys = new EntityKey[entitySpan]; //can't reuse in this case
						}

						tmpResults.Add(o);
					}

					queryInfo.Result = tmpResults;
					if (queryInfo.CanPutToCache)
						queryInfo.ResultToCache = tmpResults;

					await (reader.NextResultAsync(cancellationToken)).ConfigureAwait(false);
				}

				await (InitializeEntitiesAndCollectionsAsync(reader, hydratedObjects, cancellationToken)).ConfigureAwait(false);

				return rowCount;
			}
		}

		/// <inheritdoc />
		public async Task ExecuteNonBatchedAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			_finalResults = await (GetResultsNonBatchedAsync(cancellationToken)).ConfigureAwait(false);
			AfterLoadCallback?.Invoke(_finalResults);
		}

		protected abstract Task<IList<TResult>> GetResultsNonBatchedAsync(CancellationToken cancellationToken);

		private async Task InitializeEntitiesAndCollectionsAsync(DbDataReader reader, List<object>[] hydratedObjects, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			for (var i = 0; i < _queryInfos.Count; i++)
			{
				var queryInfo = _queryInfos[i];
				if (queryInfo.IsResultFromCache)
					continue;
				await (queryInfo.Loader.InitializeEntitiesAndCollectionsAsync(
					hydratedObjects[i], reader, Session, queryInfo.Parameters.IsReadOnly(Session),
					queryInfo.CacheBatcher, queryInfo.Parameters.UncacheableCollections, cancellationToken)).ConfigureAwait(false);
			}
		}
	}
}

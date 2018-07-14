using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;

namespace NHibernate.Cache
{
	/// <summary>
	/// A batcher for batching operations of <see cref="ICacheConcurrencyStrategy"/>.
	/// </summary>
	public sealed partial class CacheBatcher
	{
		private readonly Dictionary<ICacheConcurrencyStrategy, CachePutBatch> _putBatches =
			new Dictionary<ICacheConcurrencyStrategy, CachePutBatch>();
		private readonly ISessionImplementor _session;

		private static readonly INHibernateLogger Log = NHibernateLogger.For(typeof(CacheBatcher));

		internal CacheBatcher(ISessionImplementor session)
		{
			_session = session;
		}

		/// <summary>
		/// Adds a put operation to the batch.
		/// </summary>
		/// <param name="persister">The entity persister.</param>
		/// <param name="data">The data to put in the cache.</param>
		internal void AddToBatch(IEntityPersister persister, CachePutData data)
		{
			if (Log.IsDebugEnabled())
			{
				Log.Debug("Adding a put operation to batch for entity {0} and key {1}", persister.EntityName, data.Key);
			}
			AddToBatch(persister.Cache, data);
		}

		/// <summary>
		/// Adds a put operation to the batch.
		/// </summary>
		/// <param name="persister">The collection persister.</param>
		/// <param name="data">The data to put in the cache.</param>
		internal void AddToBatch(ICollectionPersister persister, CachePutData data)
		{
			if (Log.IsDebugEnabled())
			{
				Log.Debug("Adding a put operation to batch for collection role {0} and key {1}", persister.Role, data.Key);
			}
			AddToBatch(persister.Cache, data);
		}

		private void AddToBatch(ICacheConcurrencyStrategy cache, CachePutData data)
		{
			if (!_putBatches.TryGetValue(cache, out var cachePutBatch))
			{
				cachePutBatch = new CachePutBatch(_session, cache);
				_putBatches.Add(cache, cachePutBatch);
			}

			cachePutBatch.Add(data);
		}

		/// <summary>
		/// Executes the pending batches.
		/// </summary>
		internal void ExecuteBatch()
		{
			if (_putBatches.Count == 0)
			{
				return;
			}

			try
			{
				Stopwatch duration = null;
				if (Log.IsDebugEnabled())
				{
					duration = Stopwatch.StartNew();
				}

				foreach (var batch in _putBatches.Values)
				{
					batch.Execute();
				}

				if (Log.IsDebugEnabled() && duration != null)
				{
					Log.Debug(
						"ExecuteBatch for {0} batches totaling {1} keys took {2} ms",
						_putBatches.Count, _putBatches.Values.Sum(b => b.BatchSize),
						duration.ElapsedMilliseconds);
				}
			}
			finally
			{
				Cleanup();
			}
		}

		/// <summary>
		/// Cleans up the current batch.
		/// </summary>
		internal void Cleanup()
		{
			_putBatches.Clear();
		}
	}
}

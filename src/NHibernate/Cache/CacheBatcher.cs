using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using NHibernate.Cache.Access;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;

namespace NHibernate.Cache
{
	/// <summary>
	/// A batcher for batching operations of <see cref="ICacheConcurrencyStrategy"/>, where the batch size is retrived
	/// from an <see cref="IEntityPersister"/> or <see cref="ICollectionPersister"/>.
	/// When a different persister or a different operation is added to the batch, the current batch will be executed.
	/// </summary>
	internal partial class CacheBatcher
	{
		private CachePutBatch _putBatch;
		private ISessionImplementor _session;
		private AbstractCacheBatch _currentBatch;
		private object _currentPersister;

		protected static readonly INHibernateLogger Log = NHibernateLogger.For(typeof(CacheBatcher));

		public CacheBatcher(ISessionImplementor session)
		{
			_session = session;
		}

		/// <summary>
		/// Adds a put operation to the batch. If the batch size reached the persister batch
		/// size, the batch will be executed.
		/// </summary>
		/// <param name="persister">The entity persister.</param>
		/// <param name="data">The data to put in the cache.</param>
		public void AddToBatch(IEntityPersister persister, CachePutData data)
		{
			if (ShouldExecuteBatch(persister, _putBatch))
			{
				ExecuteBatch();
				_currentPersister = persister;
				_currentBatch = _putBatch = new CachePutBatch(_session, persister.Cache);
			}
			if (Log.IsDebugEnabled())
			{
				Log.Debug("Adding a put operation to batch for entity {0} and key {1}", persister.EntityName, data.Key);
			}
			_putBatch.Add(data);
		}

		/// <summary>
		/// Adds a put operation to the batch. If the batch size reached the persister batch
		/// size, the batch will be executed.
		/// </summary>
		/// <param name="persister">The collection persister.</param>
		/// <param name="data">The data to put in the cache.</param>
		public void AddToBatch(ICollectionPersister persister, CachePutData data)
		{
			if (ShouldExecuteBatch(persister, _putBatch))
			{
				ExecuteBatch();
				_currentPersister = persister;
				_currentBatch = _putBatch = new CachePutBatch(_session, persister.Cache);
			}
			if (Log.IsDebugEnabled())
			{
				Log.Debug("Adding a put operation to batch for collection role {0} and key {1}", persister.Role, data.Key);
			}
			_putBatch.Add(data);
		}

		/// <summary>
		/// Executes the current batch.
		/// </summary>
		public void ExecuteBatch()
		{
			if (_currentBatch == null || _currentBatch.BatchSize == 0)
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
				_currentBatch.Execute();
				if (Log.IsDebugEnabled() && duration != null)
				{
					Log.Debug("ExecuteBatch for {0} keys took {1} ms", _currentBatch.BatchSize, duration.ElapsedMilliseconds);
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
		public void Cleanup()
		{
			_putBatch = null;

			_currentBatch = null;
			_currentPersister = null;
		}

		private bool ShouldExecuteBatch(IEntityPersister persister, AbstractCacheBatch batch)
		{
			return batch != _currentBatch || _currentPersister != persister ||
				   _currentBatch.BatchSize >= persister.GetBatchSize();
		}

		private bool ShouldExecuteBatch(ICollectionPersister persister, AbstractCacheBatch batch)
		{
			return batch != _currentBatch || _currentPersister != persister ||
				   _currentBatch.BatchSize >= persister.GetBatchSize();
		}
	}
}

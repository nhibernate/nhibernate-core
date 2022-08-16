using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Entity;

namespace NHibernate.Loader.Entity
{
	internal partial class DynamicBatchingEntityLoader : AbstractBatchingEntityLoader
	{
		readonly int _maxBatchSize;
		readonly IUniqueEntityLoader _singleKeyLoader;
		readonly DynamicEntityLoader _dynamicEntityLoader;

		public DynamicBatchingEntityLoader(IOuterJoinLoadable persister, int maxBatchSize, LockMode lockMode, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters) : base(persister)
		{
			this._maxBatchSize = maxBatchSize;
			this._singleKeyLoader = new EntityLoader(persister, 1, lockMode, factory, enabledFilters);
			this._dynamicEntityLoader = new DynamicEntityLoader(persister, lockMode, factory, enabledFilters);
		}

		public override object Load(object id, object optionalObject, ISessionImplementor session)
		{
			object[] batch = session.PersistenceContext.BatchFetchQueue.GetEntityBatch(Persister, id, _maxBatchSize);

			var numberOfIds = DynamicBatchingHelper.GetIdsToLoad(batch, out var idsToLoad);
			if (numberOfIds <= 1)
			{
				return _singleKeyLoader.Load(id, optionalObject, session);
			}

			QueryParameters qp = BuildQueryParameters(id, idsToLoad, optionalObject);
			IList results = _dynamicEntityLoader.DoEntityBatchFetch(session, qp);
			return GetObjectFromList(results, id, session);
		}
	}
}

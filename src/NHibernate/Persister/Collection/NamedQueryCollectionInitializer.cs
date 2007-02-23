using System;
using log4net;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Loader.Collection;

namespace NHibernate.Persister.Collection
{
	public class NamedQueryCollectionInitializer : ICollectionInitializer
	{
		private readonly string queryName;
		private readonly ICollectionPersister persister;

		private static readonly ILog log = LogManager.GetLogger(typeof(NamedQueryCollectionInitializer));

		public NamedQueryCollectionInitializer(string queryName, ICollectionPersister persister)
		{
			this.queryName = queryName;
			this.persister = persister;
		}

		public void Initialize(object key, ISessionImplementor session)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug(
					"initializing collection: " +
					persister.Role +
					" using named query: " +
					queryName
					);
			}

			//TODO: is there a more elegant way than downcasting?
			AbstractQueryImpl query = (AbstractQueryImpl) session.GetNamedSQLQuery(queryName);
			if (query.HasNamedParameters)
			{
				query.SetParameter(
					query.NamedParameters[0],
					key,
					persister.KeyType
					);
			}
			else
			{
				query.SetParameter(0, key, persister.KeyType);
			}
			query.SetCollectionKey(key)
				.SetFlushMode(FlushMode.Never)
				.List();
		}
	}
}
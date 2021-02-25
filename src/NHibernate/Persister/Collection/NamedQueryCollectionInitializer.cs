using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Loader.Collection;

namespace NHibernate.Persister.Collection
{
	public partial class NamedQueryCollectionInitializer : ICollectionInitializer
	{
		private readonly string queryName;
		private readonly ICollectionPersister persister;

		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(NamedQueryCollectionInitializer));

		public NamedQueryCollectionInitializer(string queryName, ICollectionPersister persister)
		{
			this.queryName = queryName;
			this.persister = persister;
		}

		public void Initialize(object key, ISessionImplementor session)
		{
			if (log.IsDebugEnabled())
			{
				log.Debug("initializing collection: {0} using named query: {1}", persister.Role, queryName);
			}

			//TODO: is there a more elegant way than downcasting?
			AbstractQueryImpl query = (AbstractQueryImpl) session.GetNamedSQLQuery(queryName);
			if (query.NamedParameters.Length > 0)
			{
				query.SetParameter(query.NamedParameters[0], key, persister.KeyType);
			}
			else
			{
				query.SetParameter(0, key, persister.KeyType);
			}
			query.SetCollectionKey(key).SetFlushMode(FlushMode.Manual).List();
		}
	}
}

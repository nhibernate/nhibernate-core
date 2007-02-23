using System;
using log4net;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Loader.Entity;

namespace NHibernate.Persister.Entity
{
	public class NamedQueryLoader : IUniqueEntityLoader
	{
		private readonly string queryName;
		private readonly IEntityPersister persister;

		private static readonly ILog log = LogManager.GetLogger(typeof(NamedQueryLoader));

		public NamedQueryLoader(string queryName, IEntityPersister persister)
			: base()
		{
			this.queryName = queryName;
			this.persister = persister;
		}

		public object Load(object id, object optionalObject, ISessionImplementor session)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug(
					"loading entity: " + persister.MappedClass.FullName +
					" using named query: " + queryName
					);
			}

			AbstractQueryImpl query = (AbstractQueryImpl) session.GetNamedQuery(queryName);
			if (query.HasNamedParameters)
			{
				query.SetParameter(
					query.NamedParameters[0],
					id,
					persister.IdentifierType
					);
			}
			else
			{
				query.SetParameter(0, id, persister.IdentifierType);
			}
			query.SetOptionalId(id);
			query.SetOptionalEntityName(persister.MappedClass);
			query.SetOptionalObject(optionalObject);
			query.SetFlushMode(FlushMode.Never);
			query.List();

			// now look up the object we are really interested in!
			// (this lets us correctly handle proxies and multi-row
			// or multi-column queries)
			return session.GetEntity(new EntityKey(id, persister));
		}
	}
}
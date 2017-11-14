
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Loader.Entity;

namespace NHibernate.Persister.Entity
{
	/// <summary> 
	/// Not really a <tt>Loader</tt>, just a wrapper around a named query.
	/// </summary>
	public partial class NamedQueryLoader : IUniqueEntityLoader
	{
		private readonly string queryName;
		private readonly IEntityPersister persister;

		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(NamedQueryLoader));

		public NamedQueryLoader(string queryName, IEntityPersister persister)
		{
			this.queryName = queryName;
			this.persister = persister;
		}

		public object Load(object id, object optionalObject, ISessionImplementor session)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug(string.Format("loading entity: {0} using named query: {1}", persister.EntityName, queryName));
			}

			AbstractQueryImpl query = (AbstractQueryImpl) session.GetNamedQuery(queryName);
			if (query.HasNamedParameters)
			{
				query.SetParameter(query.NamedParameters[0], id, persister.IdentifierType);
			}
			else
			{
				query.SetParameter(0, id, persister.IdentifierType);
			}
			query.SetOptionalId(id);
			query.SetOptionalEntityName(persister.EntityName);
			query.SetOptionalObject(optionalObject);
			query.SetFlushMode(FlushMode.Manual);
			query.List();

			// now look up the object we are really interested in!
			// (this lets us correctly handle proxies and multi-row
			// or multi-column queries)
			return session.PersistenceContext.GetEntity(session.GenerateEntityKey(id, persister));
		}
	}
}
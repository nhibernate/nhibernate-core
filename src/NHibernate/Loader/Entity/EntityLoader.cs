using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Loader.Entity
{
	/// <summary>
	/// Load an entity using outerjoin fetching to fetch associated entities.
	/// </summary>
	/// <remarks>
	/// The <see cref="IEntityPersister"/> must implement <see cref="ILoadable" />. For other entities,
	/// create a customized subclass of <see cref="Loader" />.
	/// </remarks>
	public class EntityLoader : AbstractEntityLoader
	{
		private readonly bool batchLoader;

		public EntityLoader(IOuterJoinLoadable persister, LockMode lockMode, ISessionFactoryImplementor factory,
			IDictionary<string, IFilter> enabledFilters)
			: this(persister, 1, lockMode, factory, enabledFilters) {}

		public EntityLoader(IOuterJoinLoadable persister, int batchSize, LockMode lockMode,
			ISessionFactoryImplementor factory,
			IDictionary<string, IFilter> enabledFilters)
			: this(persister, persister.IdentifierColumnNames, persister.IdentifierType, batchSize, lockMode, factory, enabledFilters) {}

		public EntityLoader(IOuterJoinLoadable persister, string[] uniqueKey, IType uniqueKeyType,
			int batchSize, LockMode lockMode, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
			: base(persister, uniqueKeyType, factory, enabledFilters)
		{
			JoinWalker walker = new EntityJoinWalker(persister, uniqueKey, batchSize, lockMode, factory, enabledFilters);
			InitFromWalker(walker);

			PostInstantiate();

			batchLoader = batchSize > 1;

			log.Debug("Static select for entity " + entityName + ": " + SqlString);
		}

		public object LoadByUniqueKey(ISessionImplementor session, object key)
		{
			return Load(session, key, null, null);
		}

		protected override bool IsSingleRowLoader
		{
			get { return !batchLoader; }
		}
	}
}
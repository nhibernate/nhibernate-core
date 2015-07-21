using System.Collections.Generic;

using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.SqlCommand;

namespace NHibernate.Loader.Collection
{
	/// <summary> 
	/// Loads a collection of values or a many-to-many association.
	/// </summary>
	/// <remarks>
	/// The collection persister must implement <seealso cref="IQueryableCollection"/>. For
	/// other collections, create a customized subclass of <seealso cref="Loader"/>
	/// </remarks>
	/// <seealso cref="OneToManyLoader"/>
	public class BasicCollectionLoader : CollectionLoader
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof (BasicCollectionLoader));

		public BasicCollectionLoader(IQueryableCollection collectionPersister, ISessionFactoryImplementor session,
		                             IDictionary<string, IFilter> enabledFilters)
			: this(collectionPersister, 1, session, enabledFilters) {}

		public BasicCollectionLoader(IQueryableCollection collectionPersister, int batchSize,
		                             ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
			: this(collectionPersister, batchSize, null, factory, enabledFilters) {}

		protected BasicCollectionLoader(IQueryableCollection collectionPersister, int batchSize, SqlString subquery,
		                                ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
			: base(collectionPersister, factory, enabledFilters)
		{
			InitializeFromWalker(collectionPersister, subquery, batchSize, enabledFilters, factory);
		}

		protected virtual void InitializeFromWalker(IQueryableCollection collectionPersister, SqlString subquery, int batchSize, IDictionary<string, IFilter> enabledFilters, ISessionFactoryImplementor factory)
		{
			JoinWalker walker = new BasicCollectionJoinWalker(collectionPersister, batchSize, subquery, factory, enabledFilters);
			InitFromWalker(walker);

			PostInstantiate();

			log.Debug("Static select for collection " + collectionPersister.Role + ": " + SqlString);
		}
	}
}
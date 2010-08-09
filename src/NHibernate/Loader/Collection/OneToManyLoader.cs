using System.Collections.Generic;

using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.SqlCommand;

namespace NHibernate.Loader.Collection
{
	/// <summary>
	/// Loads one-to-many associations
	/// </summary>
	/// <remarks>
	/// The collection persister must implement <see cref="IQueryableCollection" />.
	/// For other collections, create a customized subclass of <see cref="Loader" />.
	/// </remarks>
	/// <seealso cref="BasicCollectionLoader"/>
	public class OneToManyLoader : CollectionLoader
	{
		private static readonly ILogger log = LoggerProvider.LoggerFor(typeof (OneToManyLoader));

		public OneToManyLoader(IQueryableCollection oneToManyPersister, ISessionFactoryImplementor session,
		                       IDictionary<string, IFilter> enabledFilters)
			: this(oneToManyPersister, 1, session, enabledFilters) {}

		public OneToManyLoader(IQueryableCollection oneToManyPersister, int batchSize, ISessionFactoryImplementor factory,
		                       IDictionary<string, IFilter> enabledFilters)
			: this(oneToManyPersister, batchSize, null, factory, enabledFilters) {}

		public OneToManyLoader(IQueryableCollection oneToManyPersister, int batchSize, SqlString subquery,
		                       ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
			: base(oneToManyPersister, factory, enabledFilters)
		{
			JoinWalker walker = new OneToManyJoinWalker(oneToManyPersister, batchSize, subquery, factory, enabledFilters);
			InitFromWalker(walker);

			PostInstantiate();

			log.Debug("Static select for one-to-many " + oneToManyPersister.Role + ": " + SqlString);
		}
	}
}
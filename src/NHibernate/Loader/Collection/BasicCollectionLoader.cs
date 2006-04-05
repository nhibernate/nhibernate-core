using System;
using System.Collections;

using log4net;

using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Loader.Collection
{
	public class BasicCollectionLoader : CollectionLoader
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( BasicCollectionLoader ) );

		public BasicCollectionLoader(
			IQueryableCollection collectionPersister, 
			ISessionFactoryImplementor session, 
			IDictionary enabledFilters)
			: this( collectionPersister, 1, session, enabledFilters)
		{
		}

		public BasicCollectionLoader(
			IQueryableCollection collectionPersister, 
			int batchSize, 
			ISessionFactoryImplementor factory, 
			IDictionary enabledFilters)
			: this(collectionPersister, batchSize, null, factory, enabledFilters)
		{
		}
	
		protected BasicCollectionLoader(
			IQueryableCollection collectionPersister, 
			int batchSize, 
			String subquery, 
			ISessionFactoryImplementor factory, 
			IDictionary enabledFilters)
			: base(collectionPersister, factory, enabledFilters)
		{
			JoinWalker walker = new BasicCollectionJoinWalker(
				collectionPersister, 
				batchSize, 
				subquery, 
				factory, 
				enabledFilters
				);
			InitFromWalker( walker );
			
			PostInstantiate();

			log.Debug( "Static select for collection " + collectionPersister.Role + ": " + SqlString );
		}
	}
}

using System;

using log4net;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Loader
{
	/// <summary>
	/// "Batch" loads collections, using multiple foreign key values in the SQL Where clause
	/// </summary>
	public class BatchingCollectionInitializer : ICollectionInitializer
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( BatchingCollectionInitializer ) );

		private readonly Loader nonBatchLoader;
		private readonly Loader batchLoader;
		private readonly Loader smallBatchLoader;
		private readonly int batchSize;
		private readonly int smallBatchSize;
		private readonly ICollectionPersister collectionPersister;

		public BatchingCollectionInitializer( ICollectionPersister collPersister, int batchSize, Loader batchLoader, int smallBatchSize, Loader smallBatchLoader, Loader nonBatchLoader )
		{
			this.batchLoader = batchLoader;
			this.nonBatchLoader = nonBatchLoader;
			this.batchSize = batchSize;
			this.collectionPersister = collPersister;
			this.smallBatchLoader = smallBatchLoader;
			this.smallBatchSize = smallBatchSize;
		}

		public void Initialize( object id, ISessionImplementor session )
		{
			object[ ] batch = session.GetCollectionBatch( collectionPersister, id, batchSize );
			if( smallBatchSize == 1 || batch[ smallBatchSize - 1 ] == null )
			{
				nonBatchLoader.LoadCollection( session, id, collectionPersister.KeyType );
			}
			else if( batch[ batchSize - 1 ] == null )
			{
				if( log.IsDebugEnabled )
				{
					log.Debug( string.Format( "batch loading collection role (small batch): {0}", collectionPersister.Role ) );
				}
				object[ ] smallBatch = new object[smallBatchSize];
				Array.Copy( batch, 0, smallBatch, 0, smallBatchSize );
				smallBatchLoader.LoadCollectionBatch( session, smallBatch, collectionPersister.KeyType );
				log.Debug( "done batch load" );
			}
			else
			{
				if( log.IsDebugEnabled )
				{
					log.Debug( string.Format( "batch loading collection role (small batch): {0}", collectionPersister.Role ) );
				}
				batchLoader.LoadCollectionBatch( session, batch, collectionPersister.KeyType );
				log.Debug( "done batch load" );
			}
		}
	}
}
using NHibernate.Collection;
using NHibernate.Engine;
using log4net;

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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collPersister"></param>
		/// <param name="batchSize"></param>
		/// <param name="batchLoader"></param>
		/// <param name="smallBatchSize"></param>
		/// <param name="smallBatchLoader"></param>
		/// <param name="nonBatchLoader"></param>
		public BatchingCollectionInitializer( ICollectionPersister collPersister, int batchSize, Loader batchLoader, int smallBatchSize, Loader smallBatchLoader, Loader nonBatchLoader)
		{
			this.batchLoader = batchLoader;
			this.nonBatchLoader = nonBatchLoader;
			this.batchSize = batchSize;
			this.collectionPersister = collPersister;
			this.smallBatchLoader = smallBatchLoader;
			this.smallBatchSize = smallBatchSize;
		}

		#region ICollectionInitializer Members
		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="collection"></param>
		/// <param name="owner"></param>
		/// <param name="session"></param>
		public void Initialize(object id, PersistentCollection collection, object owner, ISessionImplementor session)
		{
			// TODO:  Add BatchingCollectionInitializer.Initialize implementation
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="session"></param>
		public void Initialize( object id, ISessionImplementor session )
		{
			object[] batch = session.GetCollectionBatch( collectionPersister, id, batchSize );
			if ( smallBatchSize == 1 || batch[ smallBatchSize - 1 ] == null )
			{
				nonBatchLoader.LoadCollection( session, id, collectionPersister.KeyType );
			}
			else if ( batch[ batchSize - 1 ] == null )
			{
				if ( log.IsDebugEnabled )
				{
					log.Debug( string.Format( "batch loading collection role (small batch): {0} ", collectionPersister.Role ) );
				}
				// TODO: (2.1) BatchLoader - Copy the array from batch
				object[] smallBatch = new object[ smallBatchSize ];
				smallBatchLoader.LoadCollectionBatch( session, smallBatch, collectionPersister.KeyType );
				log.Debug( "done batch load");
			}
			else
			{
				if ( log.IsDebugEnabled )
				{
					log.Debug( string.Format( "batch loading collection role (small batch): {0} ", collectionPersister.Role ) );
				}
				batchLoader.LoadCollectionBatch( session, batch, collectionPersister.KeyType );
				log.Debug( "done batch load");
			}
		}
		#endregion
	}
}

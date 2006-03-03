using System;
using System.Collections;

using log4net;

using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Loader.Entity
{
	/// <summary>
	/// "Batch" loads entities, using multiple primary key values in the
	/// SQL <c>where</c> clause.
	/// </summary>
	public class BatchingEntityLoader : IUniqueEntityLoader
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( BatchingEntityLoader ) );

		private readonly Loader nonBatchLoader;
		private readonly Loader batchLoader;
		private readonly Loader smallBatchLoader;
		private readonly int batchSize;
		private readonly int smallBatchSize;
		private readonly IEntityPersister persister;
		private readonly IType idType;

		public BatchingEntityLoader( IEntityPersister persister, int batchSize, Loader batchLoader, int smallBatchSize, Loader smallBatchLoader, Loader nonBatchLoader )
		{
			this.batchLoader = batchLoader;
			this.nonBatchLoader = nonBatchLoader;
			this.batchSize = batchSize;
			this.persister = persister;
			this.smallBatchLoader = smallBatchLoader;
			this.smallBatchSize = smallBatchSize;
			idType = persister.IdentifierType;
		}

		public object Load( ISessionImplementor session, object id, object optionalObject )
		{
			object[ ] batch = session.GetClassBatch( persister.MappedClass, id, batchSize );
			IList list;
			if( smallBatchSize == 1 || batch[ smallBatchSize - 1 ] == null )
			{
				return ( ( IUniqueEntityLoader ) nonBatchLoader ).Load( session, id, optionalObject );
			}
			else if( batch[ batchSize - 1 ] == null )
			{
				if( log.IsDebugEnabled )
				{
					log.Debug( "batch loading entity (smaller batch): " + persister.MappedClass.Name );
				}
				object[ ] smallBatch = new object[smallBatchSize];
				Array.Copy( batch, 0, smallBatch, 0, smallBatchSize );
				list = smallBatchLoader.LoadEntityBatch( session, smallBatch, idType, optionalObject, id );
				log.Debug( "done batch load" );
			}
			else
			{
				if( log.IsDebugEnabled )
				{
					log.Debug( "batch loading entity: " + persister.MappedClass.Name );
				}
				list = batchLoader.LoadEntityBatch( session, batch, idType, optionalObject, id );
				log.Debug( "done batch load" );
			}

			// get the right object from the list ... would it be easier to just call getEntity() ??
			foreach( object obj in list )
			{
				if( id.Equals( session.GetEntityIdentifier( obj ) ) )
				{
					return obj;
				}
			}
			return null;
		}

	}
}
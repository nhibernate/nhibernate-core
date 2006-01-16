using System;
using System.Collections;
using System.Data;

using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Loader;
using NHibernate.SqlCommand;

namespace NHibernate.Collection
{
	/// <summary>
	/// Collection persister for collections of values and many-to-many associations.
	/// </summary>
	public class BasicCollectionPersister : AbstractCollectionPersister
	{
		public BasicCollectionPersister( Mapping.Collection collection, ISessionFactoryImplementor factory ) : base( collection, factory )
		{
		}

		/// <summary>
		/// Generate the SQL DELETE that deletes all rows
		/// </summary>
		/// <returns></returns>
		protected override SqlString GenerateDeleteString()
		{
			SqlDeleteBuilder delete = new SqlDeleteBuilder( factory )
				.SetTableName( qualifiedTableName )
				.SetIdentityColumn( KeyColumnNames, KeyType );
			if( HasWhere )
			{
				delete.AddWhereFragment( sqlWhereString );
			}
			return delete.ToSqlString();
		}

		/// <summary>
		/// Generate the SQL INSERT that creates a new row
		/// </summary>
		/// <returns></returns>
		protected override SqlString GenerateInsertRowString()
		{
			SqlInsertBuilder insert = new SqlInsertBuilder( factory )
				.SetTableName( qualifiedTableName )
				.AddColumn( KeyColumnNames, KeyType );
			if( HasIndex )
			{
				insert.AddColumn( IndexColumnNames, IndexType );
			}
			if( hasIdentifier )
			{
				insert.AddColumn( new string[ ] {identifierColumnName}, IdentifierType );
			}
			insert.AddColumn( ElementColumnNames, ElementType );

			return insert.ToSqlString();
		}

		/// <summary>
		/// Generate the SQL UPDATE that updates a row
		/// </summary>
		/// <returns></returns>
		protected override SqlString GenerateUpdateRowString()
		{
			SqlUpdateBuilder update = new SqlUpdateBuilder( factory )
				.SetTableName( qualifiedTableName )
				.AddColumns( ElementColumnNames, ElementType );
			if( hasIdentifier )
			{
				update.AddWhereFragment( rowSelectColumnNames, rowSelectType, " = " );
			}
			else
			{
				update.AddWhereFragment( KeyColumnNames, KeyType, " = " )
					.AddWhereFragment( rowSelectColumnNames, rowSelectType, " = " );
			}

			return update.ToSqlString();
		}

		/// <summary>
		/// Generate the SQL DELETE that deletes a particular row
		/// </summary>
		/// <returns></returns>
		protected override SqlString GenerateDeleteRowString()
		{
			SqlDeleteBuilder delete = new SqlDeleteBuilder( factory );
			delete.SetTableName( qualifiedTableName );
			if( hasIdentifier )
			{
				delete.AddWhereFragment( rowSelectColumnNames, rowSelectType, " = " );
			}
			else
			{
				delete.AddWhereFragment( KeyColumnNames, KeyType, " = " )
					.AddWhereFragment( rowSelectColumnNames, rowSelectType, " = " );
			}

			return delete.ToSqlString();
		}

		public override bool ConsumesAlias()
		{
			return false;
		}

		public override bool IsOneToMany
		{
			get { return false; }
		}

		public override bool IsManyToMany
		{
			get { return ElementType.IsEntityType; }
		}

		protected override int DoUpdateRows( object id, IPersistentCollection collection, ISessionImplementor session )
		{
			try
			{
				IDbCommand st = null;
				IEnumerable entries = collection.Entries();
				try
				{
					int i = 0;
					int count = 0;
					foreach( object entry in entries )
					{
						if( collection.NeedsUpdating( entry, i, ElementType ) )
						{
							if( st == null )
							{
								st = session.Batcher.PrepareBatchCommand( SqlUpdateRowString );
							}
							if( !hasIdentifier )
							{
								WriteKey( st, id, true, session );
							}
							collection.WriteTo( st, this, entry, i, true );
							session.Batcher.AddToBatch( 1 );
							count++;
						}
						i++;
					}
					return count;
				}
				catch( Exception e )
				{
					//TODO: change to SqlException
					// NB This calls cmd.Dispose
					session.Batcher.AbortBatch( e );
					throw;
				}
			}
			catch( HibernateException )
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch( Exception sqle )
			{
				throw Convert( sqle, "could not update collection rows: " + MessageHelper.InfoString( this, id ) );
			}
		}

		/// <summary>
		/// Create the <see cref="CollectionLoader" />
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		protected override ICollectionInitializer CreateCollectionInitializer( ISessionFactoryImplementor factory )
		{
			Loader.Loader nonbatchLoader = new CollectionLoader( this, factory );
			if( batchSize > 1 )
			{
				Loader.Loader batchLoader = new CollectionLoader( this, batchSize, factory );
				int smallBatchSize = ( int ) Math.Round( Math.Sqrt( batchSize ) );
				Loader.Loader smallBatchLoader = new CollectionLoader( this, smallBatchSize, factory );
				// the strategy for choosing batch or single load:
				return new BatchingCollectionInitializer( this, batchSize, batchLoader, smallBatchSize, smallBatchLoader, nonbatchLoader );
			}
			else
			{
				// don't do batch loading
				return ( ICollectionInitializer ) nonbatchLoader;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="innerJoin"></param>
		/// <param name="includeSubclasses"></param>
		/// <returns></returns>
		public override SqlString FromJoinFragment( string alias, bool innerJoin, bool includeSubclasses )
		{
			return SqlString.Empty;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="innerJoin"></param>
		/// <param name="includeSubclasses"></param>
		/// <returns></returns>
		public override SqlString WhereJoinFragment( string alias, bool innerJoin, bool includeSubclasses )
		{
			return SqlString.Empty;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="suffix"></param>
		/// <param name="includeCollectionColumns"></param>
		/// <returns></returns>
		public override SqlString SelectFragment( string alias, string suffix, bool includeCollectionColumns )
		{
			return includeCollectionColumns ? SelectFragment( alias ) : SqlString.Empty;
		}
	}
}
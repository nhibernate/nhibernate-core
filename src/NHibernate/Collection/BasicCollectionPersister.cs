using System;
using System.Collections;
using System.Data;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Impl;
using NHibernate.Loader;
using NHibernate.Mapping;
using NHibernate.Metadata;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Collection
{
	/// <summary>
	/// Collection persister for collections of values and many-to-many associations.
	/// </summary>
	public class BasicCollectionPersister : AbstractCollectionPersister
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="datastore"></param>
		/// <param name="factory"></param>
		public BasicCollectionPersister( Mapping.Collection collection, Configuration datastore, ISessionFactoryImplementor factory ) : base( collection, datastore, factory )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override SqlString GenerateDeleteString( )
		{
			SqlDeleteBuilder delete = new SqlDeleteBuilder( factory );
			delete.SetTableName( QualifiedTableName );
			if( HasIdentifier )
			{
				delete.AddWhereFragment( RowSelectColumnNames, RowSelectType, " = " );
			}
			else
			{
				delete.AddWhereFragment( KeyColumnNames, KeyType, " = " )
					.AddWhereFragment( RowSelectColumnNames, RowSelectType, " = " );
			}

			return delete.ToSqlString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override SqlString GenerateInsertRowString( )
		{
			SqlInsertBuilder insert = new SqlInsertBuilder( factory );
			insert.SetTableName( QualifiedTableName )
				.AddColumn( KeyColumnNames, KeyType );
			if( HasIndex )
			{
				insert.AddColumn( IndexColumnNames, IndexType );
			}
			if( HasIdentifier )
			{
				insert.AddColumn( new string[ ] { IdentifierColumnName }, IdentifierType );
			}
			insert.AddColumn( ElementColumnNames, ElementType );

			return insert.ToSqlString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override SqlString GenerateUpdateRowString( )
		{
			SqlUpdateBuilder update = new SqlUpdateBuilder( factory );
			update.SetTableName( QualifiedTableName )
				.AddColumns( ElementColumnNames, ElementType );
			if( HasIdentifier )
			{
				update.AddWhereFragment( RowSelectColumnNames, RowSelectType, " = " );
			}
			else
			{
				update.AddWhereFragment( KeyColumnNames, KeyType, " = " )
					.AddWhereFragment( RowSelectColumnNames, RowSelectType, " = " );
			}

			return update.ToSqlString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override SqlString GenerateDeleteRowString( )
		{
			SqlDeleteBuilder delete = new SqlDeleteBuilder( factory );
			delete.SetTableName( QualifiedTableName );
			if( HasIdentifier )
			{
				delete.AddWhereFragment( RowSelectColumnNames, RowSelectType, " = " );
			}
			else
			{
				delete.AddWhereFragment( KeyColumnNames, KeyType, " = " )
					.AddWhereFragment( RowSelectColumnNames, RowSelectType, " = " );
			}

			return delete.ToSqlString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override bool ConsumesAlias( )
		{
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool IsOneToMany
		{
			get { return false; }
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool IsManyToMany
		{
			get { return ElementType.IsEntityType; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="collection"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		protected int DoUpdateRows( object id, PersistentCollection collection, ISessionImplementor session )
		{
			try
			{
				IDbCommand st = null;
				ICollection entries = collection.Entries();
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
							WriteKey( st, id, false, session );
							if ( !HasIdentifier )
							{
								WriteKey( st, id, true, session );
							}
							collection.WriteTo( st, this, entry, i, false );
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
					session.Batcher.AbortBatch( e );
					throw;
				}
			}
			catch ( Exception )
			{
				//TODO: change to SqlException
				throw;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		protected override ICollectionInitializer CreateCollectionInitializer( ISessionFactoryImplementor factory )
		{
			// Don't worry about batching for now
			// TODO: Uncomment when we implement CollectionLoader
			//return (ICollectionInitializer) new CollectionLoader( this, factory );
			return null;
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
			return null;
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
			return null;
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
			return includeCollectionColumns ? SelectFragment( alias ) : null;
		}
	}
}

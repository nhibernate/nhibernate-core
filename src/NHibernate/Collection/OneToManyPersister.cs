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
	/// Summary description for OneToManyPersister.
	/// </summary>
	public class OneToManyPersister : AbstractCollectionPersister
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="datastore"></param>
		/// <param name="factory"></param>
		public OneToManyPersister( Mapping.Collection collection, Configuration datastore, ISessionFactoryImplementor factory ) : base( collection, datastore, factory )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override SqlString GenerateDeleteString( )
		{
			SqlUpdateBuilder update = new SqlUpdateBuilder( factory )
				.SetTableName( QualifiedTableName )
				.AddColumns( KeyColumnNames, "null" )
				.SetIdentityColumn( KeyColumnNames, KeyType );
			if( HasIndex )
			{
				update.AddColumns( IndexColumnNames, "null" );
			}
			if( HasWhere )
			{
				update.AddWhereFragment( Where );
			}

			return update.ToSqlString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override SqlString GenerateInsertRowString( )
		{
			SqlUpdateBuilder update = new SqlUpdateBuilder( factory );
			update.SetTableName( QualifiedTableName )
				.AddColumns( KeyColumnNames, KeyType )
				.SetIdentityColumn( ElementColumnNames, ElementType );
			if( HasIndex )
			{
				update.AddColumns( IndexColumnNames, IndexType );
			}

			return update.ToSqlString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override SqlString GenerateUpdateRowString( )
		{
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override SqlString GenerateDeleteRowString( )
		{
			SqlUpdateBuilder update = new SqlUpdateBuilder( factory );
			update.SetTableName( QualifiedTableName )
				.AddColumns( KeyColumnNames, "null" );

			if( HasIndex )
			{
				update.AddColumns( IndexColumnNames, "null" );
			}

			if( HasIdentifier )
			{
				update.AddWhereFragment( RowSelectColumnNames, RowSelectType, " = " );
			}
			else
			{
				update.AddWhereFragment( KeyColumnNames, KeyType, " = " );
				update.AddWhereFragment( RowSelectColumnNames, RowSelectType, " = " );
			}

			return update.ToSqlString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override bool ConsumesAlias( )
		{
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool IsOneToMany
		{
			get { return true; }
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool IsManyToMany
		{
			get { return false; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="collection"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		protected override int DoUpdateRows( object id, PersistentCollection collection, ISessionImplementor session )
		{
			// we finish all the "removes" first to take care of possible unique 
			// constraints and so that we can take better advantage of batching

			IDbCommand st;
			ICollection entries;
			int i;
			int count;

			try
			{
				// update removed rows fks to null
				count = 0;
				try
				{
					st = null;
					i = 0;
					entries = collection.Entries();
					foreach( object entry in entries )
					{
						if( collection.NeedsUpdating( entry, i, ElementType ) ) // will still be issued when it used to be null
						{
							if( st == null )
							{
								st = session.Batcher.PrepareBatchCommand( SqlDeleteRowString );
							}
							WriteKey( st, id, false, session );
							WriteIndex( st, collection.GetIndex( entry, i ), false, session );
							session.Batcher.AddToBatch( -1 );
							count++;
						}
						i++;
					}
				}
				catch( Exception e )
				{
					//TODO: change to SqlException
					session.Batcher.AbortBatch( e );
					throw;
				}

				// now update all changed or added rows fks
				count = 0;
				try
				{
					st = null;
					i = 0;
					entries = collection.Entries();
					foreach( object entry in entries )
					{
						if( collection.NeedsUpdating( entry, i, ElementType ) ) // will still be issued when it used to be null
						{
							if( st == null )
							{
								st = session.Batcher.PrepareBatchCommand( SqlInsertRowString );
							}
							WriteKey( st, id, false, session );
							collection.WriteTo( st, this, entry, i, false );
							session.Batcher.AddToBatch( 1 );
							count++;
						}
						i++;
					}
				}
				catch( Exception e )
				{
					//TODO: change to SqlException
					session.Batcher.AbortBatch( e );
					throw;
				}
				return count;
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
		/// <param name="alias"></param>
		/// <param name="suffix"></param>
		/// <param name="includeCollectionColumns"></param>
		/// <returns></returns>
		public override SqlString SelectFragment( string alias, string suffix, bool includeCollectionColumns )
		{
			IOuterJoinLoadable ojl = (IOuterJoinLoadable) ElementPersister;

			if ( includeCollectionColumns )
			{
				// Super impl will ignore suffix for collection columns!
				return SelectFragment( alias ).Append( StringHelper.CommaSpace ).Append( ojl.SelectFragment( alias, suffix ) );
			}
			else
			{
				// Use suffix for the entity columns.
				return ojl.SelectFragment( alias, suffix );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		protected override ICollectionInitializer CreateCollectionInitializer( ISessionFactoryImplementor factory )
		{
			Loader.Loader nonbatchLoader = new OneToManyLoader( this, factory );

			/*
			if ( batchSize > 1 )
			{
				Loader batchLoader = new OneToManyLoader( this, batchSize, factory );
				int smallBatchSize = (int) Math.Round( Math.Sqrt( batchSize ) );
				Loader smallBatchLoader = new OneToManyLoader( this, smallBatchSize, factory );
				// the strategy for choosing batch or single load
				return new BatchingCollectionInitializer( this, batchSize, batchLoader, smallBatchSize, smallBatchLoader, nonbatchLoader );
			}
			else
			{
				// don't to batch loading
				return (ICollectionInitializer) nonbatchLoader;
			}
			*/

			// Don't worry about batching for now
			return nonbatchLoader as ICollectionInitializer;
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
			return ( (IJoinable) ElementPersister).FromJoinFragment( alias, innerJoin, includeSubclasses );
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
			return ( (IJoinable) ElementPersister).WhereJoinFragment( alias, innerJoin, includeSubclasses );
		}
	}
}

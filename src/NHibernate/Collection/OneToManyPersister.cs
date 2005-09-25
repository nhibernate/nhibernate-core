using System;
using System.Collections;
using System.Data;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Loader;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Collection
{
	/// <summary>
	/// Summary description for OneToManyPersister.
	/// </summary>
	public class OneToManyPersister : AbstractCollectionPersister
	{
		public OneToManyPersister( Mapping.Collection collection, ISessionFactoryImplementor factory ) : base( collection, factory )
		{
		}

		/// <summary>
		/// Generate the SQL UPDATE that updates all the foreign keys to null
		/// </summary>
		/// <returns></returns>
		protected override SqlString GenerateDeleteString( )
		{
			SqlUpdateBuilder update = new SqlUpdateBuilder( factory )
				.SetTableName( qualifiedTableName )
				.AddColumns( KeyColumnNames, "null" )
				.SetIdentityColumn( KeyColumnNames, KeyType );
			if( HasIndex )
			{
				update.AddColumns( IndexColumnNames, "null" );
			}
			if( HasWhere )
			{
				update.AddWhereFragment( sqlWhereString );
			}

			return update.ToSqlString();
		}

		/// <summary>
		/// Generate the SQL UPDATE that updates a foreign key to a value
		/// </summary>
		/// <returns></returns>
		protected override SqlString GenerateInsertRowString( )
		{
			SqlUpdateBuilder update = new SqlUpdateBuilder( factory );
			update.SetTableName( qualifiedTableName )
				.AddColumns( KeyColumnNames, KeyType )
				.SetIdentityColumn( ElementColumnNames, ElementType );
			if( HasIndex )
			{
				update.AddColumns( IndexColumnNames, IndexType );
			}
			//identifier collections not supported for 1-to-many 

			return update.ToSqlString();
		}

		/// <summary>
		/// Not needed for one-to-many association
		/// </summary>
		/// <returns></returns>
		protected override SqlString GenerateUpdateRowString( )
		{
			return null;
		}

		/// <summary>
		/// Generate the SQL UPDATE that updates a particular row's foreign
		/// key to null
		/// </summary>
		/// <returns></returns>
		protected override SqlString GenerateDeleteRowString( )
		{
			SqlUpdateBuilder update = new SqlUpdateBuilder( factory );
			update.SetTableName( qualifiedTableName )
				.AddColumns( KeyColumnNames, "null" );

			if( HasIndex )
			{
				update.AddColumns( IndexColumnNames, "null" );
			}

			if( hasIdentifier )
			{
				update.AddWhereFragment( rowSelectColumnNames, rowSelectType, " = " );
			}
			else
			{
				update.AddWhereFragment( KeyColumnNames, KeyType, " = " );
				update.AddWhereFragment( rowSelectColumnNames, rowSelectType, " = " );
			}

			return update.ToSqlString();
		}

		public override bool ConsumesAlias( )
		{
			return true;
		}

		public override bool IsOneToMany
		{
			get { return true; }
		}

		public override bool IsManyToMany
		{
			get { return false; }
		}

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
		/// Create the <see cref="OneToManyLoader" />
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		protected override ICollectionInitializer CreateCollectionInitializer( ISessionFactoryImplementor factory )
		{
			Loader.Loader nonbatchLoader = new OneToManyLoader( this, factory );

			if ( batchSize > 1 )
			{
				Loader.Loader batchLoader = new OneToManyLoader( this, batchSize, factory );
				int smallBatchSize = (int) Math.Round( Math.Sqrt( batchSize ) );
				Loader.Loader smallBatchLoader = new OneToManyLoader( this, smallBatchSize, factory );
				// the strategy for choosing batch or single load
				return new BatchingCollectionInitializer( this, batchSize, batchLoader, smallBatchSize, smallBatchLoader, nonbatchLoader );
			}
			else
			{
				// don't to batch loading
				return (ICollectionInitializer) nonbatchLoader;
			}
		}

		public override SqlString FromJoinFragment( string alias, bool innerJoin, bool includeSubclasses )
		{
			return ( (IJoinable) ElementPersister).FromJoinFragment( alias, innerJoin, includeSubclasses );
		}

		public override SqlString WhereJoinFragment( string alias, bool innerJoin, bool includeSubclasses )
		{
			return ( (IJoinable) ElementPersister).WhereJoinFragment( alias, innerJoin, includeSubclasses );
		}
	}
}

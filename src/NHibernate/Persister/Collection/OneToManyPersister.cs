using System;
using System.Collections;
using System.Data;
using System.Text;
using NHibernate.AdoNet;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Loader.Collection;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Persister.Collection
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
		protected override SqlCommandInfo GenerateDeleteString( )
		{
			SqlUpdateBuilder update = new SqlUpdateBuilder( Factory )
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

			return update.ToSqlCommandInfo();
		}

		/// <summary>
		/// Generate the SQL UPDATE that updates a foreign key to a value
		/// </summary>
		/// <returns></returns>
		protected override SqlCommandInfo GenerateInsertRowString( )
		{
			SqlUpdateBuilder update = new SqlUpdateBuilder( Factory );
			update.SetTableName( qualifiedTableName )
				.AddColumns( KeyColumnNames, KeyType )
				.SetIdentityColumn( ElementColumnNames, ElementType );
			if( HasIndex )
			{
				update.AddColumns( IndexColumnNames, IndexType );
			}
			//identifier collections not supported for 1-to-many 

			return update.ToSqlCommandInfo();
		}

		/// <summary>
		/// Not needed for one-to-many association
		/// </summary>
		/// <returns></returns>
		protected override SqlCommandInfo GenerateUpdateRowString( )
		{
			return null;
		}

		/// <summary>
		/// Generate the SQL UPDATE that updates a particular row's foreign
		/// key to null
		/// </summary>
		/// <returns></returns>
		protected override SqlCommandInfo GenerateDeleteRowString( )
		{
			SqlUpdateBuilder update = new SqlUpdateBuilder( Factory );
			update.SetTableName( qualifiedTableName )
				.AddColumns( KeyColumnNames, "null" );

			if( HasIndex /* && TODO H3: !indexContainsFormula */ )
			{
				update.AddColumns( IndexColumnNames, "null" );
			}

			update.AddWhereFragment( KeyColumnNames, KeyType, " = " )
				.AddWhereFragment( ElementColumnNames, ElementType, " = " );

			return update.ToSqlCommandInfo();
		}

		public override bool ConsumesEntityAlias( )
		{
			return true;
		}

		public override bool ConsumesCollectionAlias()
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

		protected override int DoUpdateRows( object id, IPersistentCollection collection, ISessionImplementor session )
		{
			// we finish all the "removes" first to take care of possible unique 
			// constraints and so that we can take better advantage of batching

			IDbCommand st = null;
			IEnumerable entries;
			int i;
			try
			{
				// update removed rows fks to null
				IExpectation expectation = Expectations.AppropriateExpectation(DeleteCheckStyle);
				bool useBatch = expectation.CanBeBatched;
				int count = 0;
				try
				{
					i = 0;
					entries = collection.Entries();
					int offset = 0;
					foreach( object entry in entries )
					{
						if( collection.NeedsUpdating( entry, i, ElementType ) ) // will still be issued when it used to be null
						{
							if (useBatch)
							{
								if (st == null)
								{
									st = session.Batcher.PrepareBatchCommand(SqlDeleteRowString.CommandType, SqlDeleteRowString.Text, SqlDeleteRowString.ParameterTypes);
								}
							}
							else
							{
								st = session.Batcher.PrepareCommand(
									SqlDeleteRowString.CommandType,
									SqlDeleteRowString.Text,
									SqlDeleteRowString.ParameterTypes);
							}
							//offset += expectation.Prepare(st, Factory.ConnectionProvider.Driver);
							int loc = WriteKey( st, id, offset, session );
							WriteElementToWhere( st, collection.GetSnapshotElement( entry, i ), loc, session );
							if (useBatch)
							{
								session.Batcher.AddToBatch(expectation);
							}
							else
							{
								expectation.VerifyOutcomeNonBatched(session.Batcher.ExecuteNonQuery(st), st);
							}
							count++;
						}
						i++;
					}
				}
				catch( Exception e )
				{
					if (useBatch)
					{
						session.Batcher.AbortBatch(e);
					}
					throw;
				}
				finally
				{
					if (!useBatch && st != null)
					{
						session.Batcher.CloseCommand(st, null);
					}
				}

				// now update all changed or added rows fks
				count = 0;
				st = null;

				try
				{
					expectation = Expectations.AppropriateExpectation(DeleteCheckStyle);
					useBatch = expectation.CanBeBatched;
					i = 0;
					entries = collection.Entries();
					foreach( object entry in entries )
					{
						int offset = 0;
						if( collection.NeedsUpdating( entry, i, ElementType ) ) // will still be issued when it used to be null
						{
							if (useBatch)
							{
								if (st == null)
								{
									st = session.Batcher.PrepareBatchCommand(SqlInsertRowString.CommandType, SqlInsertRowString.Text, SqlInsertRowString.ParameterTypes);
								}
							}
							else
							{
								st = session.Batcher.PrepareCommand(
									SqlInsertRowString.CommandType,
									SqlInsertRowString.Text,
									SqlInsertRowString.ParameterTypes);
							}

							//offset += expectation.Prepare(st, Factory.ConnectionProvider.Driver);
							int loc = WriteKey( st, id, offset, session );
							if( HasIndex /* TODO H3: && !indexContainsFormula*/ )
							{
								loc = WriteIndexToWhere( st, collection.GetIndex( entry, i ), loc, session );
							}
							WriteElementToWhere( st, collection.GetElement( entry ), loc, session );
							if (useBatch)
							{
								session.Batcher.AddToBatch(expectation);
							}
							else
							{
								expectation.VerifyOutcomeNonBatched(session.Batcher.ExecuteNonQuery(st), st);
							}
							count++;
						}
						i++;
					}
				}
				catch( Exception e )
				{
					if (useBatch)
					{
						session.Batcher.AbortBatch(e);
					}
					throw;
				}
				finally
				{
					if (st != null)
					{
						session.Batcher.CloseCommand(st, null);
					}
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

		public override string SelectFragment(
			IJoinable rhs,
			string rhsAlias,
			string lhsAlias,
			string entitySuffix,
			string collectionSuffix,
			bool includeCollectionColumns )
		{
			StringBuilder buf = new StringBuilder();

			if ( includeCollectionColumns )
			{
				buf.Append( SelectFragment( lhsAlias,collectionSuffix ) )
					.Append( StringHelper.CommaSpace );
			}

			IOuterJoinLoadable ojl = (IOuterJoinLoadable) ElementPersister;
			return buf.Append( ojl.SelectFragment( lhsAlias, entitySuffix ) )//use suffix for the entity columns
				.ToString();
		}

		/// <summary>
		/// Create the <see cref="OneToManyLoader" />
		/// </summary>
		protected override ICollectionInitializer CreateCollectionInitializer( IDictionary enabledFilters )
		{
			return BatchingCollectionInitializer.CreateBatchingOneToManyInitializer( this, batchSize, Factory, enabledFilters );
		}

		public override SqlString FromJoinFragment( string alias, bool innerJoin, bool includeSubclasses )
		{
			return ( (IJoinable) ElementPersister).FromJoinFragment( alias, innerJoin, includeSubclasses );
		}

		public override SqlString WhereJoinFragment( string alias, bool innerJoin, bool includeSubclasses )
		{
			return ( (IJoinable) ElementPersister).WhereJoinFragment( alias, innerJoin, includeSubclasses );
		}
		
		protected override string FilterFragment(string alias)
		{
			string result = base.FilterFragment( alias );
			if ( ElementPersister is IJoinable )
			{
				result += ( ( IJoinable ) ElementPersister ).OneToManyFilterFragment( alias );
			}
			return result;
		}

	}
}

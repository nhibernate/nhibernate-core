using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using NHibernate.AdoNet;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Impl;
using NHibernate.Loader.Collection;
using NHibernate.Loader.Entity;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Persister.Collection
{
	public class OneToManyPersister : AbstractCollectionPersister
	{
		private readonly bool _cascadeDeleteEnabled;
		private readonly bool _keyIsNullable;
		private readonly bool _keyIsUpdateable;

		public OneToManyPersister(Mapping.Collection collection, ICacheConcurrencyStrategy cache, Configuration cfg, ISessionFactoryImplementor factory)
			: base(collection, cache, cfg, factory)
		{
			_cascadeDeleteEnabled = collection.Key.IsCascadeDeleteEnabled && factory.Dialect.SupportsCascadeDelete;
			_keyIsNullable = collection.Key.IsNullable;
			_keyIsUpdateable = collection.Key.IsUpdateable;
		}

		protected override bool RowDeleteEnabled
		{
			get { return _keyIsUpdateable && _keyIsNullable; }
		}

		protected override bool RowInsertEnabled
		{
			get { return _keyIsUpdateable; }
		}

		public override bool CascadeDeleteEnabled
		{
			get { return _cascadeDeleteEnabled; }
		}

		public override bool IsOneToMany
		{
			get { return true; }
		}

		public override bool IsManyToMany
		{
			get { return false; }
		}

		/// <summary>
		/// Generate the SQL UPDATE that updates all the foreign keys to null
		/// </summary>
		/// <returns></returns>
		protected override SqlCommandInfo GenerateDeleteString()
		{
			var update = new SqlUpdateBuilder(Factory.Dialect, Factory)
				.SetTableName(qualifiedTableName)
				.AddColumns(KeyColumnNames, "null")
				.SetIdentityColumn(KeyColumnNames, KeyType);

			if (HasIndex)
				update.AddColumns(IndexColumnNames, "null");

			if (HasWhere)
				update.AddWhereFragment(sqlWhereString);

			if (Factory.Settings.IsCommentsEnabled)
				update.SetComment("delete one-to-many " + Role);

			return update.ToSqlCommandInfo();
		}

		/// <summary>
		/// Generate the SQL UPDATE that updates a foreign key to a value
		/// </summary>
		/// <returns></returns>
		protected override SqlCommandInfo GenerateInsertRowString()
		{
			var update = new SqlUpdateBuilder(Factory.Dialect, Factory);

			update.SetTableName(qualifiedTableName)
				.AddColumns(KeyColumnNames, KeyType);

			if (HasIndex && !indexContainsFormula)
				update.AddColumns(IndexColumnNames, IndexType);

			//identifier collections not supported for 1-to-many 
			if (Factory.Settings.IsCommentsEnabled)
				update.SetComment("create one-to-many row " + Role);

			update.SetIdentityColumn(ElementColumnNames, ElementType);

			return update.ToSqlCommandInfo();
		}

		/// <summary>
		/// Not needed for one-to-many association
		/// </summary>
		/// <returns></returns>
		protected override SqlCommandInfo GenerateUpdateRowString()
		{
			return null;
		}

		/// <summary>
		/// Generate the SQL UPDATE that updates a particular row's foreign
		/// key to null
		/// </summary>
		/// <returns></returns>
		protected override SqlCommandInfo GenerateDeleteRowString()
		{
			var update = new SqlUpdateBuilder(Factory.Dialect, Factory);
			update.SetTableName(qualifiedTableName)
				.AddColumns(KeyColumnNames, "null");

			if (HasIndex && !indexContainsFormula)
				update.AddColumns(IndexColumnNames, "null");

			if (Factory.Settings.IsCommentsEnabled)
				update.SetComment("delete one-to-many row " + Role);

			//use a combination of foreign key columns and pk columns, since
			//the ordering of removal and addition is not guaranteed when
			//a child moves from one parent to another
			update
				.AddWhereFragment(KeyColumnNames, KeyType, " = ")
				.AddWhereFragment(ElementColumnNames, ElementType, " = ");

			return update.ToSqlCommandInfo();
		}

		public override bool ConsumesEntityAlias()
		{
			return true;
		}

		public override bool ConsumesCollectionAlias()
		{
			return true;
		}

		protected override int DoUpdateRows(object id, IPersistentCollection collection, ISessionImplementor session)
		{
			// we finish all the "removes" first to take care of possible unique 
			// constraints and so that we can take better advantage of batching
			try
			{
				const int offset = 0;
				int count = 0;

				if (RowDeleteEnabled)
				{
					IExpectation deleteExpectation = Expectations.AppropriateExpectation(DeleteCheckStyle);
					bool useBatch = deleteExpectation.CanBeBatched;
					SqlCommandInfo sql = SqlDeleteRowString;
					IDbCommand st = null;
					// update removed rows fks to null
					try
					{
						int i = 0;
						IEnumerable entries = collection.Entries(this);

						foreach (object entry in entries)
						{
							if (collection.NeedsUpdating(entry, i, ElementType))
							{
								// will still be issued when it used to be null
								if (useBatch)
								{
									st = session.Batcher.PrepareBatchCommand(SqlDeleteRowString.CommandType, sql.Text,
																			 SqlDeleteRowString.ParameterTypes);
								}
								else
								{
									st = session.Batcher.PrepareCommand(SqlDeleteRowString.CommandType, sql.Text,
																		SqlDeleteRowString.ParameterTypes);
								}

								int loc = WriteKey(st, id, offset, session);
								WriteElementToWhere(st, collection.GetSnapshotElement(entry, i), loc, session);
								if (useBatch)
								{
									session.Batcher.AddToBatch(deleteExpectation);
								}
								else
								{
									deleteExpectation.VerifyOutcomeNonBatched(session.Batcher.ExecuteNonQuery(st), st);
								}
								count++;
							}
							i++;
						}
					}
					catch (Exception e)
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
				}

				if (RowInsertEnabled)
				{
					IExpectation insertExpectation = Expectations.AppropriateExpectation(InsertCheckStyle);
					//bool callable = InsertCallable;
					bool useBatch = insertExpectation.CanBeBatched;
					SqlCommandInfo sql = SqlInsertRowString;
					IDbCommand st = null;

					// now update all changed or added rows fks
					try
					{
						int i = 0;
						IEnumerable entries = collection.Entries(this);
						foreach (object entry in entries)
						{
							if (collection.NeedsUpdating(entry, i, ElementType))
							{
								if (useBatch)
								{
									st = session.Batcher.PrepareBatchCommand(SqlInsertRowString.CommandType, sql.Text,
																			 SqlInsertRowString.ParameterTypes);
								}
								else
								{
									st = session.Batcher.PrepareCommand(SqlInsertRowString.CommandType, sql.Text,
																		SqlInsertRowString.ParameterTypes);
								}

								//offset += insertExpectation.Prepare(st, Factory.ConnectionProvider.Driver);
								int loc = WriteKey(st, id, offset, session);
								if (HasIndex && !indexContainsFormula)
								{
									loc = WriteIndexToWhere(st, collection.GetIndex(entry, i, this), loc, session);
								}
								WriteElementToWhere(st, collection.GetElement(entry), loc, session);
								if (useBatch)
								{
									session.Batcher.AddToBatch(insertExpectation);
								}
								else
								{
									insertExpectation.VerifyOutcomeNonBatched(session.Batcher.ExecuteNonQuery(st), st);
								}
								count++;
							}
							i++;
						}
					}
					catch (Exception e)
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
				}
				return count;
			}
			catch (DbException sqle)
			{
				throw ADOExceptionHelper.Convert(SQLExceptionConverter, sqle, "could not update collection rows: " + MessageHelper.InfoString(this, id));
			}
		}

		public override string SelectFragment(IJoinable rhs, string rhsAlias, string lhsAlias, string entitySuffix, string collectionSuffix, bool includeCollectionColumns)
		{
			var buf = new StringBuilder();

			if (includeCollectionColumns)
			{
				buf.Append(SelectFragment(lhsAlias, collectionSuffix)).Append(StringHelper.CommaSpace);
			}

			var ojl = (IOuterJoinLoadable)ElementPersister;
			return buf.Append(ojl.SelectFragment(lhsAlias, entitySuffix)).ToString(); //use suffix for the entity columns
		}

		protected override SelectFragment GenerateSelectFragment(string alias, string columnSuffix)
		{
			/*
			 NH-1747 FIX
			 Note: the fix for NH-1747 may cause some problem when the same ColumnName is used to represent two different properties in the same hierarchy
			 In all our test it happened only for NHibernate.Test.Legacy.MultiTableTest.MultiTableCollections
			 The mapping Multi.hbm.xml had a column named "parent" to represent the Key of a set for the relation Lower->Top and SubMulti->SubMulti everything in the same hierarchy
			 NH TODO: Have to add an early check of same ColumnName used to represent two relations.
			*/
			var ojl = (IOuterJoinLoadable)ElementPersister;
			var selectFragment = new SelectFragment(Dialect).SetSuffix(columnSuffix);
			var columnNames = KeyColumnNames;
			var columnAliases = KeyColumnAliases;
			for (int i = 0; i < columnNames.Length; i++)
			{
				var column = columnNames[i];
				var tableAlias = ojl.GenerateTableAliasForColumn(alias, column);
				selectFragment.AddColumn(tableAlias, column, columnAliases[i]);
			}
			return selectFragment;
		}

		/// <summary>
		/// Create the <see cref="OneToManyLoader" />
		/// </summary>
		protected override ICollectionInitializer CreateCollectionInitializer(IDictionary<string, IFilter> enabledFilters)
		{
			return BatchingCollectionInitializer.CreateBatchingOneToManyInitializer(this, batchSize, Factory, enabledFilters);
		}

		public override SqlString FromJoinFragment(string alias, bool innerJoin, bool includeSubclasses)
		{
			return ((IJoinable)ElementPersister).FromJoinFragment(alias, innerJoin, includeSubclasses);
		}

		public override SqlString WhereJoinFragment(string alias, bool innerJoin, bool includeSubclasses)
		{
			return ((IJoinable)ElementPersister).WhereJoinFragment(alias, innerJoin, includeSubclasses);
		}

		public override string TableName
		{
			get { return ((IJoinable)ElementPersister).TableName; }
		}

		protected override string FilterFragment(string alias)
		{
			string result = base.FilterFragment(alias);
			var j = ElementPersister as IJoinable;
			if (j != null)
				result += j.OneToManyFilterFragment(alias);
			return result;
		}

		protected override ICollectionInitializer CreateSubselectInitializer(SubselectFetch subselect, ISessionImplementor session)
		{
			return new SubselectOneToManyLoader(
				this,
				subselect.ToSubselectString(CollectionType.LHSPropertyName),
				subselect.Result,
				subselect.QueryParameters, session.Factory,
				session.EnabledFilters);
		}

		public override object GetElementByIndex(object key, object index, ISessionImplementor session, object owner)
		{
			return new CollectionElementLoader(this, Factory, session.EnabledFilters).LoadElement(session, key, IncrementIndexByBase(index)) ?? NotFoundObject;
		}

		#region NH Specific

		protected override SqlCommandInfo GenerateIdentityInsertRowString()
		{
			throw new NotSupportedException("Identity insert is not needed for one-to-many association");
		}

		#endregion
	}
}
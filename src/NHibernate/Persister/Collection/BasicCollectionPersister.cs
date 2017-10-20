using System;
using System.Collections;
using System.Data.Common;
using NHibernate.AdoNet;
using NHibernate.Cache;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Impl;
using NHibernate.Loader.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;
using System.Collections.Generic;

namespace NHibernate.Persister.Collection
{
	/// <summary>
	/// Collection persister for collections of values and many-to-many associations.
	/// </summary>
	public partial class BasicCollectionPersister : AbstractCollectionPersister
	{
		public BasicCollectionPersister(Mapping.Collection collection, ICacheConcurrencyStrategy cache, ISessionFactoryImplementor factory) 
			: base(collection, cache, factory) { }

		public override bool CascadeDeleteEnabled
		{
			get { return false; }
		}

		public override bool IsOneToMany
		{
			get { return false; }
		}

		public override bool IsManyToMany
		{
			get { return ElementType.IsEntityType; }
		}

		/// <summary>
		/// Generate the SQL DELETE that deletes all rows
		/// </summary>
		/// <returns></returns>
		protected override SqlCommandInfo GenerateDeleteString()
		{
			SqlDeleteBuilder delete = new SqlDeleteBuilder(Factory.Dialect, Factory)
				.SetTableName(qualifiedTableName)
				.SetIdentityColumn(KeyColumnNames, KeyType);
			if (HasWhere)
				delete.AddWhereFragment(sqlWhereString);

			if (Factory.Settings.IsCommentsEnabled)
				delete.SetComment("delete collection " + Role);

			return delete.ToSqlCommandInfo();
		}

		/// <summary>
		/// Generate the SQL INSERT that creates a new row
		/// </summary>
		/// <returns></returns>
		protected override SqlCommandInfo GenerateInsertRowString()
		{
			SqlInsertBuilder insert = new SqlInsertBuilder(Factory)
				.SetTableName(qualifiedTableName)
				.AddColumns(KeyColumnNames, null, KeyType);
			
			if (hasIdentifier)
				insert.AddColumns(new string[] {IdentifierColumnName}, null, IdentifierType);

			if (HasIndex)
				insert.AddColumns(IndexColumnNames, indexColumnIsSettable, IndexType);

			if (Factory.Settings.IsCommentsEnabled)
				insert.SetComment("insert collection row " + Role);

			insert.AddColumns(ElementColumnNames, elementColumnIsSettable, ElementType);

			return insert.ToSqlCommandInfo();
		}

		/// <summary>
		/// Generate the SQL UPDATE that updates a row
		/// </summary>
		/// <returns></returns>
		protected override SqlCommandInfo GenerateUpdateRowString()
		{
			SqlUpdateBuilder update = new SqlUpdateBuilder(Factory.Dialect, Factory)
				.SetTableName(qualifiedTableName)
				.AddColumns(ElementColumnNames, elementColumnIsSettable, ElementType);
			if (hasIdentifier)
			{
				update.AddWhereFragment(new string[] { IdentifierColumnName }, IdentifierType, " = ");
			}
			else if (HasIndex && !indexContainsFormula)
			{
				update.AddWhereFragment(KeyColumnNames, KeyType, " = ")
					.AddWhereFragment(IndexColumnNames, IndexType, " = ");
			}
			else
			{
				string[] cnames = ArrayHelper.Join(KeyColumnNames, ElementColumnNames, elementColumnIsInPrimaryKey);
				SqlType[] ctypes = ArrayHelper.Join(KeyType.SqlTypes(Factory), ElementType.SqlTypes(Factory), elementColumnIsInPrimaryKey);
				update.AddWhereFragment(cnames, ctypes, " = ");
			}

			if (Factory.Settings.IsCommentsEnabled)
				update.SetComment("update collection row " + Role);

			return update.ToSqlCommandInfo();
		}

		/// <summary>
		/// Generate the SQL DELETE that deletes a particular row
		/// </summary>
		/// <returns></returns>
		protected override SqlCommandInfo GenerateDeleteRowString()
		{
			SqlDeleteBuilder delete = new SqlDeleteBuilder(Factory.Dialect, Factory);
			delete.SetTableName(qualifiedTableName);
			if (hasIdentifier)
			{
				delete.AddWhereFragment(new string[] { IdentifierColumnName }, IdentifierType, " = ");
			}
			else if (HasIndex && !indexContainsFormula)
			{
				delete
					.AddWhereFragment(KeyColumnNames, KeyType, " = ")
					.AddWhereFragment(IndexColumnNames, IndexType, " = ");
			}
			else
			{
				string[] cnames = ArrayHelper.Join(KeyColumnNames, ElementColumnNames, elementColumnIsInPrimaryKey);
				SqlType[] ctypes = ArrayHelper.Join(KeyType.SqlTypes(Factory), ElementType.SqlTypes(Factory), elementColumnIsInPrimaryKey);
				delete.AddWhereFragment(cnames, ctypes, " = ");
			}

			if (Factory.Settings.IsCommentsEnabled)
				delete.SetComment("delete collection row " + Role);

			return delete.ToSqlCommandInfo();
		}

		public override bool ConsumesEntityAlias()
		{
			return false;
		}

		public override bool ConsumesCollectionAlias()
		{
			return true;
		}

		protected override int DoUpdateRows(object id, IPersistentCollection collection, ISessionImplementor session)
		{
			if (ArrayHelper.IsAllFalse(elementColumnIsSettable)) return 0;

			try
			{
				DbCommand st = null;
				IExpectation expectation = Expectations.AppropriateExpectation(UpdateCheckStyle);
				//bool callable = UpdateCallable;
				bool useBatch = expectation.CanBeBatched;
				IEnumerable entries = collection.Entries(this);
				int i = 0;
				int count = 0;
				foreach (object entry in entries)
				{
					if (collection.NeedsUpdating(entry, i, ElementType))
					{
						int offset = 0;
						if (useBatch)
						{
							if (st == null)
							{
								st =
									session.Batcher.PrepareBatchCommand(SqlUpdateRowString.CommandType, SqlUpdateRowString.Text,
																		SqlUpdateRowString.ParameterTypes);
							}
						}
						else
						{
							st =
								session.Batcher.PrepareCommand(SqlUpdateRowString.CommandType, SqlUpdateRowString.Text,
															   SqlUpdateRowString.ParameterTypes);
						}

						try
						{
							//offset += expectation.Prepare(st, Factory.ConnectionProvider.Driver);

							int loc = WriteElement(st, collection.GetElement(entry), offset, session);
							if (hasIdentifier)
							{
								WriteIdentifier(st, collection.GetIdentifier(entry, i), loc, session);
							}
							else
							{
								loc = WriteKey(st, id, loc, session);
								if (HasIndex && !indexContainsFormula)
								{
									WriteIndexToWhere(st, collection.GetIndex(entry, i, this), loc, session);
								}
								else
								{
									WriteElementToWhere(st, collection.GetSnapshotElement(entry, i), loc, session);
								}
							}

							if (useBatch)
							{
								session.Batcher.AddToBatch(expectation);
							}
							else
							{
								expectation.VerifyOutcomeNonBatched(session.Batcher.ExecuteNonQuery(st), st);
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
							if (!useBatch)
							{
								session.Batcher.CloseCommand(st, null);
							}
						}
						count++;
					}
					i++;
				}
				return count;
			}
			catch (DbException sqle)
			{
				throw ADOExceptionHelper.Convert(SQLExceptionConverter, sqle,
												 "could not update collection rows: " + MessageHelper.CollectionInfoString(this, collection, id, session),
												 SqlUpdateRowString.Text);
			}
		}

		public override string SelectFragment(IJoinable rhs, string rhsAlias, string lhsAlias,
			string entitySuffix, string collectionSuffix, bool includeCollectionColumns)
		{
			// we need to determine the best way to know that two joinables
			// represent a single many-to-many...
			if (rhs != null && IsManyToMany && !rhs.IsCollection)
			{
				IAssociationType elementType = (IAssociationType) ElementType;
				if (rhs.Equals(elementType.GetAssociatedJoinable(Factory)))
				{
					return ManyToManySelectFragment(rhs, rhsAlias, lhsAlias, collectionSuffix);
				}
			}
			return includeCollectionColumns ? SelectFragment(lhsAlias, collectionSuffix) : string.Empty;
		}

		private string ManyToManySelectFragment(IJoinable rhs, string rhsAlias, string lhsAlias, string collectionSuffix)
		{
			SelectFragment frag = GenerateSelectFragment(lhsAlias, collectionSuffix);

			string[] _elementColumnNames = rhs.KeyColumnNames;
			frag.AddColumns(rhsAlias, _elementColumnNames, elementColumnAliases);
			AppendIndexColumns(frag, lhsAlias);
			AppendIdentifierColumns(frag, lhsAlias);

			return frag.ToSqlStringFragment(false);
		}

		/// <summary>
		/// Create the <see cref="CollectionLoader" />
		/// </summary>
		protected override ICollectionInitializer CreateCollectionInitializer(IDictionary<string, IFilter> enabledFilters)
		{
			return BatchingCollectionInitializer.CreateBatchingCollectionInitializer(this, batchSize, Factory, enabledFilters);
		}

		public override SqlString FromJoinFragment(string alias, bool innerJoin, bool includeSubclasses)
		{
			return SqlString.Empty;
		}

		public override SqlString WhereJoinFragment(string alias, bool innerJoin, bool includeSubclasses)
		{
			return SqlString.Empty;
		}

		protected override ICollectionInitializer CreateSubselectInitializer(SubselectFetch subselect, ISessionImplementor session)
		{
			return
				new SubselectCollectionLoader(this, subselect.ToSubselectString(CollectionType.LHSPropertyName), subselect.Result,
											  subselect.QueryParameters, session.Factory,
											  session.EnabledFilters);
		}

		#region NH Specific
		protected override SqlCommandInfo GenerateIdentityInsertRowString()
		{
			// NH specific to manage identity for id-bag (NH-364)
			SqlInsertBuilder insert = identityDelegate.PrepareIdentifierGeneratingInsert();
			insert.SetTableName(qualifiedTableName).AddColumns(KeyColumnNames, null, KeyType);

			if (HasIndex)
				insert.AddColumns(IndexColumnNames, null, IndexType);

			insert.AddColumns(ElementColumnNames, elementColumnIsSettable, ElementType);

			if (Factory.Settings.IsCommentsEnabled)
				insert.SetComment("insert collection row " + Role);

			return insert.ToSqlCommandInfo();
		}

		#endregion
	}
}
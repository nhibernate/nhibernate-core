using System;
using System.Text;
using System.Collections;

using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader 
{
	/// <summary>
	/// Loads one-to-many associations
	/// </summary>
	public class OneToManyLoader : OuterJoinLoader, ICollectionInitializer	
	{
		private CollectionPersister collectionPersister;
		private IType idType;
		
		protected override bool EnableJoinedFetch(bool mappingDefault, string path, string table, string[] foreignKeyColumns)
		{
			return mappingDefault && (
				!table.Equals(collectionPersister.QualifiedTableName) ||
				!ArrayHelper.Equals(foreignKeyColumns, collectionPersister.KeyColumnNames)
			);
		}

		public OneToManyLoader(CollectionPersister collPersister, ISessionFactoryImplementor factory) : base ( factory.Dialect ) 
		{
			collectionPersister = collPersister;
			idType = collectionPersister.KeyType;

			ILoadable persister = (ILoadable) factory.GetPersister(
				((EntityType) collPersister.ElementType).PersistentClass);

			string alias = ToAlias(collectionPersister.QualifiedTableName, 0);
			
			SqlString whereSqlString = null;

			if (collectionPersister.HasWhere) 
				whereSqlString = new SqlString(collectionPersister.GetSQLWhereString(alias));
				
			IList associations = WalkTree(persister, alias, factory);

			int joins=associations.Count;
			Suffixes = new string[joins+1];
			for (int i=0; i<=joins; i++) Suffixes[i] = (joins==0) ? String.Empty : i.ToString() + StringHelper.Underscore;


			JoinFragment ojf = OuterJoins(associations);

			SqlSelectBuilder selectBuilder = new SqlSelectBuilder(factory);
			
			selectBuilder.SetSelectClause(
				collectionPersister.SelectClauseFragment(alias) +
				(joins==0 ? String.Empty : "," + SelectString(associations) ) +
				", " +
				SelectString( persister, alias, Suffixes[joins] )
				);


			selectBuilder.SetFromClause(
				persister.FromTableFragment(alias).Append(
					persister.FromJoinFragment(alias, true, true)
				)
			);

			selectBuilder.SetWhereClause(alias, collectionPersister.KeyColumnNames, collectionPersister.KeyType);
			if(collectionPersister.HasWhere) selectBuilder.AddWhereClause(whereSqlString);

			selectBuilder.SetOuterJoins(
				ojf.ToFromFragmentString,
				ojf.ToWhereFragmentString.Append(
					persister.WhereJoinFragment(alias, true, true)
				)
			);

			if(collectionPersister.HasOrdering) selectBuilder.SetOrderByClause( collectionPersister.GetSQLOrderByString(alias) );

			this.SqlString = selectBuilder.ToSqlString();


			Persisters = new ILoadable[joins+1];
			LockModeArray = CreateLockModeArray(joins+1, LockMode.None);
			for (int i=0; i<joins; i++) 
			{
				Persisters[i] = ((OuterJoinableAssociation) associations[i]).Subpersister;
			}
			Persisters[joins] = persister;
			
			PostInstantiate();
		}

		protected override CollectionPersister CollectionPersister {
			get { return collectionPersister; }
		}

		public void Initialize(object id, PersistentCollection collection, object owner, ISessionImplementor session) {
			LoadCollection(session, id, idType, owner, collection);
		}
	}
}

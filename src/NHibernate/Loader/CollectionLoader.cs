using System;
using System.Collections;

using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader
{
	/// <summary>
	/// Loads a collection of values or a many-to-many association
	/// </summary>
	/// <remarks>
	/// The collection persister must implement <see cref="IQueryableCollection" />. For
	/// other collections, create a customized subclass of <see cref="Loader" />.
	/// <seealso cref="OneToManyLoader" />
	/// </remarks>
	public class CollectionLoader : OuterJoinLoader, ICollectionInitializer
	{
		private readonly IQueryableCollection collectionPersister;
		private readonly IType keyType;

		public CollectionLoader( IQueryableCollection persister, ISessionFactoryImplementor factory )
			: this( persister, 1, factory )
		{
		}

		public CollectionLoader( IQueryableCollection persister, int batchSize, ISessionFactoryImplementor factory )
			: base( factory.Dialect )
		{
			this.collectionPersister = persister;
			this.keyType = persister.KeyType;

			string alias = GenerateRootAlias( persister.Role );
			IList associations = WalkCollectionTree( persister, alias, factory );

			InitStatementString( persister, alias, associations, batchSize, factory );
			InitClassPersisters( associations );

			PostInstantiate();
		}

		private void InitClassPersisters( IList associations )
		{
			int joins = associations.Count;
			lockModeArray = CreateLockModeArray( joins, LockMode.None );

			classPersisters = new ILoadable[joins];
			Owners = new int[joins];

			int i = 0;
			foreach( OuterJoinableAssociation oj in associations )
			{
				classPersisters[ i ] = ( ILoadable ) oj.Joinable;
				Owners[ i ] = ToOwner( oj, joins, oj.IsOneToOne );
				i++;
			}

			if( ArrayHelper.IsAllNegative( Owners ) )
			{
				Owners = null;
			}
		}

		protected override ICollectionPersister CollectionPersister
		{
			get { return collectionPersister; }
		}

		public void Initialize( object id, ISessionImplementor session )
		{
			LoadCollection( session, id, keyType );
		}

		private void InitStatementString( IQueryableCollection persister, string alias, IList associations, int batchSize, ISessionFactoryImplementor factory )
		{
			Suffixes = GenerateSuffixes( associations.Count );

			SqlStringBuilder whereString = WhereString( factory, alias, persister.KeyColumnNames, persister.KeyType, batchSize );
			if( persister.HasWhere )
			{
				whereString
					.Add( " and " )
					.Add( persister.GetSQLWhereString( alias ) );
			}

			JoinFragment ojf = MergeOuterJoins( associations );
			SqlSelectBuilder select = new SqlSelectBuilder( factory )
				.SetSelectClause(
				persister.SelectFragment( alias ).Append(
					SelectString( associations, factory ) ).ToString()
				)
				.SetFromClause( persister.TableName, alias )
				.SetWhereClause( whereString.ToSqlString() )
				.SetOuterJoins(
				ojf.ToFromFragmentString,
				ojf.ToWhereFragmentString
				);

			if( persister.HasOrdering )
			{
				select.SetOrderByClause( persister.GetSQLOrderByString( alias ) );
			}
			SqlString = select.ToSqlString();
		}

		protected override JoinType GetJoinType(
			IAssociationType type,
			FetchMode fetchMode,
			string path,
			string table,
			string[ ] foreignKeyColumns,
			ISessionFactoryImplementor factory
			)
		{
			JoinType joinType = base.GetJoinType( type, fetchMode, path, table, foreignKeyColumns, factory );
			// We can use an inner-join for the many-to-many
			if( joinType == JoinType.LeftOuterJoin && string.Empty.Equals( path ) )
			{
				joinType = JoinType.InnerJoin;
			}
			return joinType;
		}
	}
}
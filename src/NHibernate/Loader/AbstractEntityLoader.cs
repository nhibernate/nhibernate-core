using System;
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
	/// Abstract superclass for entity loaders that use outer joins
	/// </summary>
	public abstract class AbstractEntityLoader : OuterJoinLoader
	{
		private readonly IOuterJoinLoadable persister;
		private ICollectionPersister collectionPersister;
		private int collectionOwner;
		private string alias;
		private string[ ] aliases;

		public AbstractEntityLoader( IOuterJoinLoadable persister, ISessionFactoryImplementor factory )
			: base( factory.Dialect )
		{
			this.persister = persister;
			alias = GenerateRootAlias( persister.ClassName );
		}

		protected void RenderStatement( IList associations, SqlString condition, ISessionFactoryImplementor factory )
		{
			InitStatementString( associations, condition, string.Empty, factory );
		}

		protected void AddAllToPropertySpaces( object[ ] spaces )
		{
			for( int i = 0; i < spaces.Length; i++ )
			{
				AddToPropertySpaces( spaces[ i ] );
			}
		}

		protected virtual void AddToPropertySpaces( object space )
		{
			throw new AssertionFailure( "only criteria queries need to autoflush" );
		}

		protected void InitClassPersisters( IList associations )
		{
			int joins = CountClassPersisters( associations );

			collectionOwner = -1; // if no collection found
			classPersisters = new ILoadable[ joins + 1 ];
			Owners = new int[ joins + 1 ];
			aliases = new string[ joins + 1 ];
			lockModeArray = CreateLockModeArray( joins + 1, LockMode.None );
			int i = 0;
			foreach( OuterJoinableAssociation oj in associations )
			{
				object subpersister = oj.Joinable;
				if( subpersister is ILoadable )
				{
					classPersisters[ i ] = ( ILoadable ) subpersister;
					Owners[ i ] = ToOwner( oj, joins, oj.IsOneToOne );
					aliases[ i ] = oj.Subalias;
					if( oj.JoinType == JoinType.InnerJoin )
					{
						AddAllToPropertySpaces( classPersisters[ i ].PropertySpaces );
					}
					i++;
				}
				else
				{
					IQueryableCollection collPersister = ( IQueryableCollection ) subpersister;
					// TODO: ?? suppress initialization of collections with a where condition
					if( oj.JoinType == JoinType.LeftOuterJoin )
					{
						collectionPersister = collPersister;
						collectionOwner = ToOwner( oj, joins, true );
					}
					else
					{
						AddToPropertySpaces( collPersister.CollectionSpace );
					}

					if( collPersister.IsOneToMany )
					{
						classPersisters[ i ] = ( ILoadable ) collPersister.ElementPersister;
						aliases[ i ] = oj.Subalias;
						i++;
					}
				}
			}
			classPersisters[ joins ] = persister;
			Owners[ joins ] = -1;
			aliases[ joins ] = alias;

			if( ArrayHelper.IsAllNegative( Owners ) )
			{
				Owners = null;
			}
		}

		protected void InitStatementString(
			IList associations,
			SqlString condition,
			string orderBy,
			ISessionFactoryImplementor factory )
		{
			int joins = CountClassPersisters( associations );

			Suffixes = GenerateSuffixes( joins + 1 );

			JoinFragment ojf = MergeOuterJoins( associations );

			this.SqlString = new SqlSelectBuilder( factory )
                .SetSelectClause(
					persister.SelectFragment( alias, Suffixes[ joins ] ) +
					SelectString( associations, factory )
				)
				.SetFromClause(
					persister.FromTableFragment( alias ).Append(
					persister.FromJoinFragment( alias, true, true ) )
				)
				.SetWhereClause( condition )
				.SetOuterJoins(
					ojf.ToFromFragmentString,
					ojf.ToWhereFragmentString.Append( GetWhereFragment() )
				)
				.SetOrderByClause( orderBy )
				.ToSqlString();
		}

		/// <summary>
		/// Include discriminator, don't include the class where string.
		/// </summary>
		/// <returns></returns>
		protected virtual SqlString GetWhereFragment()
		{
			return persister.WhereJoinFragment( alias, true, true );
		}

		/// <summary>
		/// Gets the <see cref="ILoadable"/> Persister.
		/// </summary>
		protected ILoadable Persister
		{
			get { return persister; }
		}

		/// <summary></summary>
		protected string Alias
		{
			get { return alias; }
			set { alias = value; }
		}

		/// <summary></summary>
		protected override ICollectionPersister CollectionPersister
		{
			get { return collectionPersister; }
		}

		/// <summary></summary>
		protected override int CollectionOwner
		{
			get { return collectionOwner; }
		}

		/// <summary></summary>
		protected string[ ] EntityAliases
		{
			get { return aliases; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="mappingDefault"></param>
		/// <param name="path"></param>
		/// <param name="table"></param>
		/// <param name="foreignKeyColumns"></param>
		/// <returns></returns>
		protected override bool IsJoinedFetchEnabled(
			IType type,
			bool mappingDefault,
			string path,
			string table,
			string[ ] foreignKeyColumns )
		{
			return mappingDefault;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.GetType().Name + " for " + Persister.ClassName;
		}
	}
}
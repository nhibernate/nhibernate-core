using System;
using System.Collections;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Loader
{
	/// <summary></summary>
	public class AbstractEntityLoader : OuterJoinLoader
	{
		private ILoadable persister;
		private ICollectionPersister collectionPersister;
		private int collectionOwner;
		private string alias;
		private string[] aliases;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="factory"></param>
		public AbstractEntityLoader( ILoadable persister, ISessionFactoryImplementor factory ) : base( factory.Dialect )
		{
			this.persister = persister;
			alias = ToAlias( persister.ClassName, 0 );
		}

		/// <summary></summary>
		protected string Alias
		{
			get { return alias; }
		}

		/// <summary>
		/// Gets the <see cref="ILoadable"/> Persister.
		/// </summary>
		protected ILoadable Persister
		{
			get { return persister; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="selectBuilder"></param>
		/// <param name="factory"></param>
		protected void RenderStatement( SqlSelectBuilder selectBuilder, ISessionFactoryImplementor factory )
		{
			RenderStatement( selectBuilder, String.Empty, factory );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="selectBuilder"></param>
		/// <param name="orderBy"></param>
		/// <param name="factory"></param>
		protected void RenderStatement( SqlSelectBuilder selectBuilder, string orderBy, ISessionFactoryImplementor factory )
		{
			IList associations = WalkTree( persister, alias, factory );

			int joins = associations.Count;
			Suffixes = new string[joins + 1];
			for( int i = 0; i <= joins; i++ )
			{
				Suffixes[ i ] = ( joins == 0 ) ? String.Empty : i.ToString() + StringHelper.Underscore;
			}

			JoinFragment ojf = OuterJoins( associations );

			selectBuilder.SetSelectClause(
				( joins == 0 ? String.Empty : SelectString( associations ) + "," ) +
					SelectString( persister, alias, Suffixes[ joins ] )
				)
				.SetFromClause
				(
				persister.FromTableFragment( alias ).Append(
					persister.FromJoinFragment( alias, true, true )
					)
				)
				.SetOuterJoins
				(
				ojf.ToFromFragmentString,
				ojf.ToWhereFragmentString.Append(
					UseQueryWhereFragment ?
						( ( IQueryable ) persister ).QueryWhereFragment( alias, true, true ) :
						persister.WhereJoinFragment( alias, true, true )
					)
				)
				.SetOrderByClause( orderBy );

			Persisters = new ILoadable[joins + 1];
			//			classPersisters = new ILoadable[joins+1];
			LockModeArray = CreateLockModeArray( joins + 1, LockMode.None );
			for( int i = 0; i < joins; i++ )
			{
				Persisters[ i ] = ( ( OuterJoinableAssociation ) associations[ i ] ).Subpersister;
			}
			Persisters[ joins ] = persister;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="factory"></param>
		protected void RenderStatement( SqlString condition, ISessionFactoryImplementor factory )
		{
			RenderStatement( condition, String.Empty, factory );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="orderBy"></param>
		/// <param name="factory"></param>
		protected void RenderStatement( SqlString condition, string orderBy, ISessionFactoryImplementor factory )
		{
			SqlSelectBuilder sqlBuilder = new SqlSelectBuilder( factory );

			IList associations = WalkTree( persister, alias, factory );

			int joins = associations.Count;
			Suffixes = new string[joins + 1];
			for( int i = 0; i <= joins; i++ )
			{
				Suffixes[ i ] = ( joins == 0 ) ? String.Empty : i.ToString() + StringHelper.Underscore;
			}

			JoinFragment ojf = OuterJoins( associations );
			sqlBuilder.SetSelectClause(
				( joins == 0 ? String.Empty : SelectString( associations ) + "," ) +
					SelectString( persister, alias, Suffixes[ joins ] )
				);

			sqlBuilder.SetFromClause(
				persister.FromTableFragment( alias ).Append(
					persister.FromJoinFragment( alias, true, true )
					)
				);

			sqlBuilder.AddWhereClause( condition );

			sqlBuilder.SetOuterJoins(
				ojf.ToFromFragmentString,
				ojf.ToWhereFragmentString.Append
					(
					UseQueryWhereFragment ?
						( ( IQueryable ) persister ).QueryWhereFragment( alias, true, true ) :
						persister.WhereJoinFragment( alias, true, true ) )
				);

			sqlBuilder.SetOrderByClause( orderBy );

			this.SqlString = sqlBuilder.ToSqlString();

			Persisters = new ILoadable[joins + 1];
			LockModeArray = CreateLockModeArray( joins + 1, LockMode.None );
			for( int i = 0; i < joins; i++ )
			{
				Persisters[ i ] = ( ( OuterJoinableAssociation ) associations[ i ] ).Subpersister;
			}
			Persisters[ joins ] = persister;
		}

		/// <summary>
		/// Should we sue the discriminator, to narrow the select to
		/// instances of the queried subclass?
		/// </summary>
		/// <value>False, unless overridden</value>
		protected virtual bool UseQueryWhereFragment
		{
			get { return false; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="associations"></param>
		protected void InitClassPersisters( IList associations )
		{
			int joins = CountClassPersisters( associations );

			collectionOwner = -1;	// if no collection found
			classPersisters = new ILoadable[ joins + 1 ];
			owners = new int[ joins + 1 ];
			aliases = new string[ joins + 1 ];
			LockModeArray = CreateLockModeArray( joins + 1, LockMode.None );
			int i = 0;
			foreach( OuterJoinableAssociation oj in associations )
			{
				object subpersister = oj.Joinable;
				if ( subpersister is ILoadable )
				{
					classPersisters[ i ] = (ILoadable) subpersister;
					owners[ i ] = ToOwner( oj, joins, oj.IsOneToOne );
					aliases[ i ] = oj.Subalias;
					if ( oj.JoinType == JoinType.InnerJoin )
					{
						AddAllToPropertySpaces( classPersisters[ i ].PropertySpaces );
					}
					i++;
				}
				else 
				{
					IQueryableCollection collPersister = (IQueryableCollection) subpersister;
					// TODO: ?? suppress initialization of collections with a where condition
					if ( oj.JoinType == JoinType.LeftOuterJoin )
					{
						collectionPersister = collPersister;
						collectionOwner = ToOwner( oj, joins, true );
					}
					else
					{
						AddToPropertySpaces( collPersister.CollectionSpace ) ;
					}

					if ( collPersister.IsOneToMany )
					{
						classPersisters[ i ] = (ILoadable) collPersister.ElementPersister;
						aliases[ i ] = oj.Subalias;
						i++;
					}
				}
			}
			classPersisters[ joins ] = persister;
			owners[ joins ] = -1;
			aliases[ joins ] = alias;

			if ( ArrayHelper.IsAllNegative( owners ) )
			{
				owners = null;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="spaces"></param>
		protected void AddAllToPropertySpaces( object[] spaces )
		{
			for ( int i = 0; i < spaces.Length; i++ )
			{
				AddToPropertySpaces( spaces[ i ] );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="space"></param>
		protected void AddToPropertySpaces( object space )
		{
			throw new NotSupportedException( "only criteria queries need to autoflush" );
		}
	}
}
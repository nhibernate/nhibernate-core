using System;
using System.Collections;
using System.Data;

using Iesi.Collections;

using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.Persister.Collection;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader.Custom
{
	public class CustomLoader : Loader
	{
		// Currently *not* cachable if autodiscover types is in effect (e.g. "select * ...")

		private readonly IType[] resultTypes;
		private readonly ILoadable[] persisters;
		private readonly ICollectionPersister[] collectionPersisters;
		private readonly ICustomQuery customQuery;
		//private IType[] discoveredTypes;
		//private string[] discoveredColumnAliases;
		//private DataTable metaData;
		private readonly string[] queryReturnAliases;

		// NH: added this because we need to alter the SQL string to
		// set parameter types when List is called.
		private SqlString sql;

		public CustomLoader(
			ICustomQuery customQuery,
			ISessionFactoryImplementor factory )
			: base( factory )
		{
			this.customQuery = customQuery;

			queryReturnAliases = customQuery.ReturnAliases;

			string[] collectionRoles = customQuery.CollectionRoles;

			if( collectionRoles == null )
			{
				collectionPersisters = null;
			}
			else
			{
				int length = collectionRoles.Length;
				collectionPersisters = new ICollectionPersister[ length ];
				for( int i = 0; i < collectionPersisters.Length; i++ )
				{
					collectionPersisters[ i ] = factory.GetCollectionPersister( collectionRoles[ i ] );
				}
			}

			System.Type[] entityNames = customQuery.EntityNames;
			persisters = new ILoadable[ entityNames.Length ];
			for( int i = 0; i < entityNames.Length; i++ )
			{
				persisters[ i ] = ( ILoadable ) factory.GetEntityPersister( entityNames[ i ] );
			}

			IType[] scalarTypes = customQuery.ScalarTypes;

			resultTypes = new IType[ entityNames.Length + ( scalarTypes == null ? 0 : scalarTypes.Length ) ];
			Array.Copy( scalarTypes, 0, resultTypes, 0, scalarTypes.Length );
			for( int i = 0; i < entityNames.Length; i++ )
			{
				resultTypes[ i + scalarTypes.Length ] = TypeFactory.ManyToOne( entityNames[ i ] );
			}

			sql = customQuery.SQL;
		}

		protected internal override SqlString SqlString
		{
			get { return sql; }
			set { sql = value; }
		}

		protected override ILoadable[] EntityPersisters
		{
			get { return persisters; }
			set { throw new NotSupportedException( "CustomLoader.set_EntityPersisters" ); }
		}

		protected override LockMode[] GetLockModes( IDictionary lockModes )
		{
			return customQuery.LockModes;
		}

		protected override ICollectionPersister[] CollectionPersisters
		{
			get { return collectionPersisters; }
		}

		protected override int[] CollectionOwners
		{
			get { return customQuery.CollectionOwner; }
		}

		public ISet QuerySpaces
		{
			get { return customQuery.QuerySpaces; }
		}

		// TODO
		//protected string QueryIdentifier
		//{
		//	get { return customQuery.SQL; }
		//}

		public IList List(
			ISessionImplementor session,
			QueryParameters queryParameters )
		{
			PopulateSqlString( queryParameters );
			return List( session, queryParameters, customQuery.QuerySpaces, resultTypes );
		}

		// Not ported: scroll
		// Not ported: getHolderInstantiator
		// Not ported: autoDiscoverTypes, getHibernateType

		protected override object GetResultColumnOrRow( object[] row, IDataReader rs, ISessionImplementor session )
		{
			IType[] scalarTypes = customQuery.ScalarTypes;
			string[] scalarColumnAliases = customQuery.ScalarColumnAliases;
			object[] resultRow;

			if( scalarTypes != null && scalarTypes.Length > 0 )
			{
				// all scalar results appear first
				resultRow = new object[ scalarTypes.Length + row.Length ];
				for( int i = 0; i < scalarTypes.Length; i++ )
				{
					resultRow[ i ] = scalarTypes[ i ].NullSafeGet( rs, scalarColumnAliases[ i ], session, null );
				}
				// then entity results
				Array.Copy( row, 0, resultRow, scalarTypes.Length, row.Length );
			}
			else
			{
				resultRow = row;
			}
			return resultRow.Length == 1 ? resultRow[ 0 ] : resultRow;
		}

		// Not ported: getReturnAliasesForTransformer()

		protected override IEntityAliases[] EntityAliases
		{
			get { return customQuery.EntityAliases; }
		}

		protected override ICollectionAliases[] CollectionAliases
		{
			get { return customQuery.CollectionAliases; }
		}

		public override int[] GetNamedParameterLocs( string name )
		{
			object loc = customQuery.NamedParameterBindPoints[ name ];
			if( loc == null )
			{
				throw new QueryException(
					"Named parameter does not appear in Query: " +
					name,
					customQuery.SQL.ToString() );
			}

			if( loc is int )
			{
				return new int[] { ( int ) loc };
			}
			else
			{
				return ArrayHelper.ToIntArray( ( IList ) loc );
			}
		}
	}
}

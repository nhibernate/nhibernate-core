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
				persisters[ i ] = ( ILoadable ) factory.GetPersister( entityNames[ i ] );
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

		/// <summary>
		/// Indicates if the SqlString has been fully populated - it goes
		/// through a 2 phase process.  The first part is the parsing of the
		/// hql and it puts in placeholders for the parameters, the second phase 
		/// puts in the actual types for the parameters using QueryParameters
		/// passed to query methods.  The completion of the second phase is
		/// when <c>isSqlStringPopulated==true</c>.
		/// </summary>
		private bool isSqlStringPopulated;

		private object prepareCommandLock = new object();

		private void PopulateSqlString( QueryParameters parameters )
		{
			lock( prepareCommandLock )
			{
				if( isSqlStringPopulated )
				{
					return;
				}

				SqlString sql = null;

				// when there is no untyped Parameters then we can avoid the need to create
				// a new sql string and just return the existing one because it is ready 
				// to be prepared and executed.
				if( SqlString.ContainsUntypedParameter == false )
				{
					sql = SqlString;
				}
				else
				{
					// holds the index of the sqlPart that should be replaced
					int sqlPartIndex = 0;

					// holds the index of the paramIndexes array that is the current position
					int paramIndex = 0;

					sql = SqlString.Clone();
					int[] paramIndexes = sql.ParameterIndexes;

					// if there are no Parameters in the SqlString then there is no reason to 
					// bother with this code.
					if( paramIndexes.Length > 0 )
					{
						for( int i = 0; i < parameters.PositionalParameterTypes.Length; i++ )
						{
							string[] colNames = new string[ parameters.PositionalParameterTypes[ i ].GetColumnSpan( Factory ) ];
							for( int j = 0; j < colNames.Length; j++ )
							{
								colNames[ j ] = "p" + paramIndex.ToString() + j.ToString();
							}

							Parameter[] sqlParameters = Parameter.GenerateParameters( Factory, colNames, parameters.PositionalParameterTypes[ i ] );

							foreach( Parameter param in sqlParameters )
							{
								sqlPartIndex = paramIndexes[ paramIndex ];
								sql.SqlParts[ sqlPartIndex ] = param;

								paramIndex++;
							}
						}

						if( parameters.NamedParameters != null && parameters.NamedParameters.Count > 0 )
						{
							// convert the named parameters to an array of types
							ArrayList paramTypeList = new ArrayList();

							foreach( DictionaryEntry e in parameters.NamedParameters )
							{
								string name = ( string ) e.Key;
								TypedValue typedval = ( TypedValue ) e.Value;
								int[] locs = GetNamedParameterLocs( name );

								for( int i = 0; i < locs.Length; i++ )
								{
									int lastInsertedIndex = paramTypeList.Count;

									int insertAt = locs[ i ];

									// need to make sure that the ArrayList is populated with null objects
									// up to the index we are about to add the values into.  An Index Out 
									// of Range exception will be thrown if we add an element at an index
									// that is greater than the Count.
									if( insertAt >= lastInsertedIndex )
									{
										for( int j = lastInsertedIndex; j <= insertAt; j++ )
										{
											paramTypeList.Add( null );
										}
									}

									paramTypeList[ insertAt ] = typedval.Type;
								}
							}

							for( int i = 0; i < paramTypeList.Count; i++ )
							{
								IType type = ( IType ) paramTypeList[ i ];
								string[] colNames = new string[ type.GetColumnSpan( Factory ) ];

								for( int j = 0; j < colNames.Length; j++ )
								{
									colNames[ j ] = "p" + paramIndex.ToString() + j.ToString();
								}

								Parameter[] sqlParameters = Parameter.GenerateParameters( Factory, colNames, type );

								foreach( Parameter param in sqlParameters )
								{
									sqlPartIndex = paramIndexes[ paramIndex ];
									sql.SqlParts[ sqlPartIndex ] = param;

									paramIndex++;
								}
							}
						}
					}
				}

				// replace the local field used by the SqlString property with the one we just built 
				// that has the correct parameters
				SqlString = sql;
				isSqlStringPopulated = true;
			}
		}
	}
}

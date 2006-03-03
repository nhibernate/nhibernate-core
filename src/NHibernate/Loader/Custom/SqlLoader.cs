using System;
using System.Collections;
using System.Data;
using System.Text;

using Iesi.Collections;

using NHibernate.Engine;
using NHibernate.Hql;
using NHibernate.Persister;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader.Custom
{
	/// <summary>
	/// Summary description for SqlLoader.
	/// </summary>
	public class SqlLoader : OuterJoinLoader
	{
		private int parameterCount = 0;
		private readonly IDictionary namedParameters = new Hashtable( 4 );
		private readonly string sqlQuery;
		private readonly IDictionary alias2Persister;
		private readonly string[ ] aliases;
		private ISet querySpaces = new HashedSet();
		private IType[ ] resultTypes;

		// NH: Remember the factory for the PopulateSqlString call.
		private IMapping factory;

		public ISet QuerySpaces
		{
			get { return querySpaces; }
		}

		public SqlLoader( string[ ] aliases, ISqlLoadable[ ] persisters, ISessionFactoryImplementor factory, string sqlQuery, ICollection additionalQuerySpaces )
			: base( factory.Dialect )
		{
			this.sqlQuery = sqlQuery;
			this.aliases = aliases;

			// Remember the factory for the PopulateSqlString call.
			this.factory = factory;

			alias2Persister = new Hashtable();
			ArrayList resultTypeList = new ArrayList();

			for( int i = 0; i < persisters.Length; i++ )
			{
				ISqlLoadable persister = persisters[ i ];
				alias2Persister.Add( aliases[ i ], persister );
				// TODO: Does not consider any other tables referenced in the query
				querySpaces.AddAll( persister.PropertySpaces );
				resultTypeList.Add( persister.Type );
			}

			if( additionalQuerySpaces != null )
			{
				querySpaces.AddAll( additionalQuerySpaces );
			}
			resultTypes = ( IType[ ] ) resultTypeList.ToArray( typeof( IType ) );

			RenderStatement( persisters );

			PostInstantiate();
		}

		private void RenderStatement( ILoadable[ ] persisters )
		{
			int loadables = persisters.Length;

			Persisters = persisters;
			Suffixes = GenerateSuffixes( loadables );
			lockModeArray = CreateLockModeArray( loadables, LockMode.None );

			SqlString = SubstituteParams( SubstituteBrackets() );
		}

		public IList List( ISessionImplementor session, QueryParameters queryParameters )
		{
			PopulateSqlString( queryParameters );
			return List( session, queryParameters, querySpaces, resultTypes );
		}

		protected override object GetResultColumnOrRow( object[ ] row, IDataReader rs, ISessionImplementor session )
		{
			if( Persisters.Length == 1 )
			{
				return row[ row.Length - 1 ];
			}
			else
			{
				return row;
			}
		}

		/// <summary>
		/// Inspired by the parsing done in TJDO
		/// TODO: Should record how many properties we have referred to - and throw exception if we don't get them all aka AbstractQueryImpl
		/// </summary>
		/// <returns></returns>
		public string SubstituteBrackets()
		{
			string sqlString = sqlQuery;

			StringBuilder result = new StringBuilder();
			int left, right;

			// replace {....} with corresponding column aliases
			for( int curr = 0; curr < sqlString.Length; curr = right + 1 )
			{
				if( ( left = sqlString.IndexOf( '{', curr ) ) < 0 )
				{
					result.Append( sqlString.Substring( curr ) );
					break;
				}

				result.Append( sqlString.Substring( curr, left - curr ) );

				if( ( right = sqlString.IndexOf( '}', left + 1 ) ) < 0 )
				{
					throw new QueryException( "Unmatched braces for alias path", sqlString );
				}

				string aliasPath = sqlString.Substring( left + 1, right - left - 1 );
				int firstDot = aliasPath.IndexOf( '.' );

				string aliasName = firstDot == -1 ? aliasPath : aliasPath.Substring( 0, firstDot );
				ISqlLoadable currentPersister = GetPersisterByResultAlias( aliasName );
				if( currentPersister == null )
				{
					// TODO: Do we throw this or pass through as per Hibernate to allow for escape sequences as per HB-898
					throw new QueryException( string.Format( "Alias [{0}] does not correspond to any of the supplied return aliases = {1}", aliasName, aliases ),
						sqlQuery );

					//result.Append( "{" + aliasPath + "}" );
					//continue;
				}
				int currentPersisterIndex = GetPersisterIndex( aliasName );

				if( firstDot == -1 )
				{
					// TODO: should this one also be aliased/quoted instead of just directly inserted ?
					result.Append( aliasPath );
				}
				else
				{
					if( aliasName != aliases[ currentPersisterIndex ] )
					{
						throw new QueryException( string.Format( "Alias [{0}] does not correspond to return alias {1}",
							aliasName, aliases[ currentPersisterIndex ] ), sqlQuery );
					}

					string propertyName = aliasPath.Substring( firstDot + 1 );

					if( "*".Equals( propertyName ) )
					{
						result.Append( currentPersister.SelectFragment( aliasName, Suffixes[ currentPersisterIndex ] ) );
					}
					else
					{
						// Here it would be nice just to be able to do result.Append( getAliasFor( currentPersister, propertyName ))
						// but that requires more exposure of the internal maps of the persister...
						// but it should be possible as propertyname should be unique for all persisters

						string[ ] columnAliases;

						/*
						if ( AbstractEntityPersister.ENTITY_CLASS.Equals( propertyName )
						{
							columnAliases = new string[1];
							columnAliases[0] = currentPersister.GetDiscriminatorAlias( suffixes[ currentPersisterIndex ] ) ;
						}
						else
						*/
						columnAliases = currentPersister.GetSubclassPropertyColumnAliases( propertyName, Suffixes[ currentPersisterIndex ] );

						if( columnAliases == null || columnAliases.Length == 0 )
						{
							throw new QueryException( string.Format( "No column name found for property [{0}]", propertyName ),
								sqlQuery );
						}

						if( columnAliases.Length != 1 )
						{
							throw new QueryException( string.Format( "SQL queries only support properties mapped to a single column. Property [{0}] is mapped to {1} columns.", propertyName, columnAliases.Length ), sqlQuery );
						}

						result.Append( columnAliases[ 0 ] );
					}
				}
			}

			// Possibly handle :something parameters for the query?
			return result.ToString();
		}

		// Heavily modified and simplified, compared to the original H2.1
		// version. The original replaced named parameters with question marks
		// (positional parameters) in the SQL string, while recording locations
		// of replaced parameters.
		//
		// This version generates a SqlString from its parameter, replacing
		// both named and positional parameters with placeholder Parameter
		// objects (that only hold the name of the parameter). Later, types
		// of the placeholder parameters are set from the actual
		// query parameters.
		private SqlString SubstituteParams( string sql )
		{
			SqlStringBuilder result = new SqlStringBuilder();

			StringTokenizer st = new StringTokenizer( sql, ParserHelper.HqlSeparators );

			int index = 0;

			foreach( string str in st )
			{
				if( str.StartsWith( ParserHelper.HqlVariablePrefix ) )
				{
					string name = str.Substring( 1 );
					result.Add( new Parameter( name ) );
					AddNamedParameter( name );
				}
				else if( "?".Equals( str ) )
				{
					string name = "p" + index.ToString();
					result.Add( new Parameter( name ) );
				}
				else
				{
					result.Add( str );
				}
			}

			return result.ToSqlString();
		}

		private int GetPersisterIndex( string aliasName )
		{
			for( int i = 0; i < aliases.Length; i++ )
			{
				if( aliasName == aliases[ i ] )
				{
					return i;
				}
			}

			return -1;
		}

		private ISqlLoadable GetPersisterByResultAlias( string aliasName )
		{
			return ( ISqlLoadable ) alias2Persister[ aliasName ];
		}

		// NAMED PARAMETER SUPPORT, copy/pasted from QueryTranslator!
		internal void AddNamedParameter( string name )
		{
			int loc = parameterCount++;
			object o = namedParameters[ name ];
			if( o == null )
			{
				namedParameters.Add( name, loc );
			}
			else if( o is int )
			{
				ArrayList list = new ArrayList( 4 );
				list.Add( o );
				list.Add( loc );
				namedParameters[ name ] = list;
			}
			else
			{
				( ( ArrayList ) o ).Add( loc );
			}
		}

		protected int[ ] GetNamedParameterLocs( string name )
		{
			object o = namedParameters[ name ];
			if( o == null )
			{
				throw new QueryException( String.Format( "Named parameter does not appear in query: {0} ", name ), sqlQuery );
			}

			if( o is int )
			{
				return new int[ ] {( int ) o};
			}
			else
			{
				return ArrayHelper.ToIntArray( ( IList ) o );
			}
		}

		/// <summary>
		/// Bind named parameters to the <c>IDbCommand</c>
		/// </summary>
		/// <param name="st">The <see cref="IDbCommand"/> that contains the parameters.</param>
		/// <param name="namedParams">The named parameters (key) and the values to set.</param>
		/// <param name="session">The <see cref="ISession"/> this Loader is using.</param>
		/// <param name="start"></param>
		/// <remarks>
		/// Assumes that all types are of span 1
		/// </remarks>
		protected override int BindNamedParameters( IDbCommand st, IDictionary namedParams, int start, ISessionImplementor session )
		{
			if( namedParameters != null )
			{
				int result = 0;
				// Assumes that types are all of span 1
				foreach( DictionaryEntry de in namedParams )
				{
					string name = ( string ) de.Key;
					TypedValue typedval = ( TypedValue ) de.Value;
					IType type = typedval.Type;
					int[ ] locs = GetNamedParameterLocs( name );
					for( int i = 0; i < locs.Length; i++ )
					{
						type.NullSafeSet( st, typedval.Value, locs[ i ] + start, session );
					}
					result += locs.Length;
				}
				return result;
			}
			else
			{
				return 0;
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
					int[ ] paramIndexes = sql.ParameterIndexes;

					// if there are no Parameters in the SqlString then there is no reason to 
					// bother with this code.
					if( paramIndexes.Length > 0 )
					{
						for( int i = 0; i < parameters.PositionalParameterTypes.Length; i++ )
						{
							string[ ] colNames = new string[parameters.PositionalParameterTypes[ i ].GetColumnSpan( factory )];
							for( int j = 0; j < colNames.Length; j++ )
							{
								colNames[ j ] = "p" + paramIndex.ToString() + j.ToString();
							}

							Parameter[ ] sqlParameters = Parameter.GenerateParameters( factory, colNames, parameters.PositionalParameterTypes[ i ] );

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
								int[ ] locs = GetNamedParameterLocs( name );

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
								string[ ] colNames = new string[type.GetColumnSpan( factory )];

								for( int j = 0; j < colNames.Length; j++ )
								{
									colNames[ j ] = "p" + paramIndex.ToString() + j.ToString();
								}

								Parameter[ ] sqlParameters = Parameter.GenerateParameters( factory, colNames, type );

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
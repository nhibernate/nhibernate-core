using System;
using System.Data;
using System.Collections;
using System.Text;

using Iesi.Collections;

using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Hql;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader
{
	/// <summary>
	/// Summary description for SqlLoader.
	/// </summary>
	public class SqlLoader : OuterJoinLoader
	{
		private int parameterCount = 0;
		private readonly IDictionary namedParameters = new Hashtable(4);
		private readonly string sqlQuery;
		private readonly IDictionary alias2Persister;
		private readonly string[] aliases;
		private ISet querySpaces = new HashedSet();
		private readonly IType[] resultTypes;

		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		/// <param name="aliases"></param>
		/// <param name="persisters"></param>
		/// <param name="factory"></param>
		/// <param name="sqlQuery"></param>
		/// <param name="additionalQuerySpaces"></param>
		/// <param name="positionalTypes"></param>
		/// <param name="namedTypes"></param>
		public SqlLoader( string[] aliases, ISqlLoadable[] persisters, ISessionFactoryImplementor factory, string sqlQuery, ICollection additionalQuerySpaces, SqlType[] positionalTypes, IDictionary namedTypes ) : base( factory.Dialect )
		{
			this.sqlQuery = sqlQuery;
			this.aliases = aliases;
			alias2Persister = new Hashtable();
			ArrayList resultTypeList = new ArrayList();

			for (int i = 0; i < persisters.Length; i++ )
			{
				ISqlLoadable persister = persisters[i];
				alias2Persister.Add( aliases[i], persister );
				// TODO: Does not consider any other tables referenced in the query
				querySpaces.AddAll( persister.PropertySpaces ) ;
				resultTypeList.Add( persister.Type );
			}

			if ( additionalQuerySpaces != null )
				querySpaces.AddAll( additionalQuerySpaces );
			resultTypes = ( IType[ ] ) resultTypeList.ToArray( typeof( IType ) );

			RenderStatement( persisters, positionalTypes, namedTypes ) ;

			PostInstantiate();
		}
		#endregion

		#region Property methods
		/// <summary>
		/// 
		/// </summary>
		public ISet QuerySpaces
		{
			get { return querySpaces; }
		}
		#endregion

		#region Private methods
		private void RenderStatement( ILoadable[] persisters, SqlType[] positionalTypes, IDictionary namedTypes )
		{
			int loadables = persisters.Length;

			Persisters = persisters;
			Suffixes = GenerateSuffixes( loadables ) ;
			LockModeArray = CreateLockModeArray( loadables, LockMode.None );

			SqlString = SubstituteParams( SubstituteBrackets( sqlQuery ), positionalTypes, namedTypes );
		}

		private SqlString SubstituteParams( string sql, SqlType[] positionalTypes, IDictionary namedTypes )
		{
			ArrayList tokens = new ArrayList(); // contains a List of strings containing Sql or SqlStrings

			StringTokenizer st = new StringTokenizer( sql, ParserHelper.HqlSeparators );

			IEnumerator enumer = st.GetEnumerator();
			int index = 0;
			while( enumer.MoveNext() ) 
			{
				string str = (string) enumer.Current;
				if ( str.StartsWith( ParserHelper.HqlVariablePrefix ) )
				{
					string name = str.Substring( 1 );
					tokens.Add( new Parameter( name, (SqlType) namedTypes[ name ] ) );
					AddNamedParameter( name );
				}
				else if ( "?".Equals( str ) )
				{
					string name = "p" + index.ToString();
					tokens.Add( new Parameter( name, positionalTypes[ index++ ] ) );
				}
				else
				{
					tokens.Add( str );
				}
			}

			return new SqlString( ( object[] ) tokens.ToArray( typeof( object ) ) );
		}

		private int PersisterIndex( string aliasName )
		{
			for ( int i = 0; i < aliases.Length; i++ )
			{
				if ( aliasName == aliases[i] )
				{
					return i;
				}
			}

			return -1;
		}

		private ISqlLoadable GetPersisterByResultAlias( string aliasName )
		{
			// NB This return the dictionary object I think not the value.
			return (ISqlLoadable) alias2Persister[ aliasName ];
		}
		#endregion

		#region Protected methods
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
			if ( namedParameters != null )
			{
				int result = 0;
				// Assumes that types are all of span 1
				foreach( DictionaryEntry de in namedParams )
				{
					string name = (string) de.Key;
					TypedValue typedval = (TypedValue) de.Value;
					IType type = typedval.Type;
					int[] locs = GetNamedParameterLocs( name );
					for ( int i = 0; i < locs.Length; i++ )
					{
						type.NullSafeSet( st, typedval.Value, locs[i] + start, session );
					}
					result += locs.Length;
				}
				return result;
			}
			else
				return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="row"></param>
		/// <param name="rs"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		protected override object GetResultColumnOrRow( object[ ] row, IDataReader rs, ISessionImplementor session )
		{
			if ( Persisters.Length == 1 )
				return row[ row.Length - 1 ];
			else
				return row;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		protected int[] GetNamedParameterLocs( string name )
		{
			object o = namedParameters[ name ];
			if ( o == null )
			{
				throw new QueryException( String.Format( "Named parameter does not appear in query: {0} ", name ) );
			}

			if ( o is int)
			{
				return new int[] { (int) o };
			}
			else
			{
				return ArrayHelper.ToIntArray( (IList) o );
			}
		}
		#endregion

		#region Public methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="queryParameters"></param>
		/// <returns></returns>
		public IList List( ISessionImplementor session, QueryParameters queryParameters )
		{
			return List( session, queryParameters, querySpaces, resultTypes ) ;
		}

		// Inspired by the parsing done in TJDO
		// TODO: Should record how many properties we have referred to - and throw exception if we don't get them all aka AbstractQueryImpl
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlQuery"></param>
		/// <returns></returns>
		public string SubstituteBrackets( string sqlQuery )
		{
			string sqlString = sqlQuery;
			StringBuilder result = new StringBuilder();
			int left, right;

			// replace {....} with corresponding column aliases
			for (int curr = 0; curr < sqlString.Length; curr = right + 1)
			{
				if ( (left = sqlString.IndexOf( '{', curr ) ) < 0 )
				{
					result.Append( sqlString.Substring( curr ) ) ;
					break;
				}

				result.Append( sqlString.Substring( curr, left - curr ) );

				if ( ( right = sqlString.IndexOf( '}', left + 1 ) ) < 0 )
				{
					throw new QueryException( "Unmatched braces for alias path" );
				}

				string aliasPath = sqlString.Substring( left + 1, right - left - 1 );
				int firstDot = aliasPath.IndexOf( '.' );

				string aliasName = firstDot == -1 ? aliasPath : aliasPath.Substring( 0, firstDot ) ;
				ISqlLoadable currentPersister = GetPersisterByResultAlias( aliasName ) ;
				if ( currentPersister == null )
				{
					// TODO: Do we throw this or pass through as per Hibernate to allow for escape sequences as per HB-898
					throw new QueryException( string.Format( "Alias [{0}] does not correspond to any of the supplied return aliases = {1}", aliasName, aliases ) );

					//result.Append( "{" + aliasPath + "}" );
					//continue;
				}
				int currentPersisterIndex = PersisterIndex( aliasName );

				if ( firstDot == -1 )
				{
					// TODO: should this one also be aliased/quoted instead of just directly inserted ?
					result.Append( aliasPath ) ;
				}
				else
				{
					if ( aliasName != aliases[ currentPersisterIndex ] )
					{
						throw new QueryException( string.Format( "Alias [{0}] does not correspond to return alias {1}.", aliasName, aliases[ currentPersisterIndex ] ) );
					}

					string propertyName = aliasPath.Substring( firstDot + 1 );

					if ( "*".Equals( propertyName ) )
					{
						result.Append( currentPersister.SelectFragment( aliasName, Suffixes[ currentPersisterIndex ] ) );
					}
					else
					{
						// Here it would be nice just to be able to do result.Append( getAliasFor( currentPersister, propertyName ))
						// but that requires more exposure of the internal maps of the persister...
						// but it should be possible as propertyname should be unique for all persisters

						string[] columnAliases;

						/*
						if ( AbstractEntityPersister.ENTITY_CLASS.Equals( propertyName )
						{
							columnAliases = new string[1];
							columnAliases[0] = currentPersister.GetDiscriminatorAlias( suffixes[ currentPersisterIndex ] ) ;
						}
						else
						*/
						columnAliases = currentPersister.GetSubclassPropertyColumnAliases( propertyName, Suffixes[ currentPersisterIndex ] );

						if ( columnAliases == null || columnAliases.Length == 0 )
						{
							throw new QueryException( string.Format( "No column name found for property [{0}]", propertyName ) );
						}

						if ( columnAliases.Length != 1 )
						{
							throw new QueryException( string.Format( "SQL queries only support properties mapped to a single column. Property [{0}] is mapped to {1} columns.", propertyName, columnAliases.Length ) );
						}

						result.Append( columnAliases[ 0 ] ) ;
					}
				}
			}

			// Possibly handle :something parameters for the query?
			return result.ToString();
		}

		// NAMED PARAMETER SUPPORT, copy/pasted from QueryTranslator
		internal void AddNamedParameter( string name )
		{
			// want the param index to start at 0 instead of 1
			//int loc = ++parameterCount;
			// TODO: 2.1 - Not sure about the loc idea - surely we need to account for positional parameters as well?
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
		#endregion
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections;
using NHibernate.Engine;
using NHibernate.Hql;
using NHibernate.Property;
using NHibernate.Proxy;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Impl
{
	/// <summary>
	/// Abstract implementation of the IQuery interface.
	/// </summary>
	public abstract class AbstractQueryImpl : IQuery
	{
		private string queryString;
		private IDictionary lockModes = new Hashtable( 2 );

		private readonly ISessionImplementor session;

		private RowSelection selection;
		private ArrayList values = new ArrayList( 4 );
		private ArrayList types = new ArrayList( 4 );
		private int positionalParameterCount = 0;
		private ISet actualNamedParameters;
		private IDictionary namedParameters = new Hashtable( 4 );
		private IDictionary namedParameterLists = new Hashtable( 4 );
		private bool cacheable;
		private string cacheRegion;
		private bool forceCacheRefresh;
		private static readonly object UNSET_PARAMETER = new object();
		private static readonly object UNSET_TYPE = new object();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="queryString"></param>
		/// <param name="session"></param>
		public AbstractQueryImpl( string queryString, ISessionImplementor session )
		{
			this.session = session;
			this.queryString = queryString;
			selection = new RowSelection();
			InitParameterBookKeeping();
		}

		/// <summary></summary>
		public string QueryString
		{
			get { return queryString; }
		}

		/// <summary></summary>
		protected IDictionary NamedParams
		{
			// NB The java one always returns a copy, so I'm going to reproduce that behaviour
			get { return new Hashtable( namedParameters ); }
		}

		protected void VerifyParameters()
		{
			if ( actualNamedParameters.Count != namedParameters.Count + namedParameterLists.Count )
			{
				Set missingParams = new ListSet( actualNamedParameters );
				missingParams.RemoveAll( namedParameterLists.Keys );
				missingParams.RemoveAll( namedParameters.Keys );
				throw new QueryException( "Not all named parameters have been set: " + CollectionPrinter.ToString( missingParams ), QueryString );
			}

			if ( positionalParameterCount != values.Count ) 
			{
				throw new QueryException( string.Format( "Not all positional parameters have been set. Expected {0}, set {1}", positionalParameterCount, values.Count ),
					QueryString );
			}

			for ( int i = 0; i < values.Count; i++ )
			{
				if ( values[i] == UNSET_PARAMETER || types[i] == UNSET_TYPE ) 
				{
					throw new QueryException( string.Format( "Not all positional parameters have been set. Found unset parameter at position {0}", i),
						QueryString );
				}
			}
		}

		protected IDictionary NamedParameterLists
		{
			get { return namedParameterLists; }
		}

		protected IList Values
		{
			get { return values; }
		}

		protected IList Types
		{
			get { return types; }
		}

		// TODO: maybe call it RowSelection ?
		public RowSelection Selection
		{
			get { return selection; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="maxResults"></param>
		/// <returns></returns>
		public IQuery SetMaxResults( int maxResults )
		{
			selection.MaxRows = maxResults;
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="timeout"></param>
		/// <returns></returns>
		public IQuery SetTimeout( int timeout )
		{
			selection.Timeout = timeout;
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fetchSize"></param>
		/// <returns></returns>
		public IQuery SetFetchSize( int fetchSize )
		{
			selection.FetchSize = fetchSize;
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="firstResult"></param>
		/// <returns></returns>
		public IQuery SetFirstResult( int firstResult )
		{
			selection.FirstRow = firstResult;
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public IQuery SetParameter( int position, object val, IType type )
		{
			if ( positionalParameterCount == 0 ) 
			{
				throw new ArgumentOutOfRangeException( string.Format( "No positional parameters in query: {0}", QueryString ) );
			}

			if ( position < 0 || position > positionalParameterCount - 1 ) 
			{
				throw new ArgumentOutOfRangeException( string.Format( "Positional parameter does not exists: {0} in query: {1}", position, QueryString ) );
			}

			int size = values.Count;
			if ( position < size )
			{
				values[ position ] = val;
				types[ position ] = type;
			}
			else
			{
				// Put guard values in for any positions before the wanted position - allows us to detect unset parameters on validate.
				for( int i = 0; i < position - size; i++ )
				{
					values.Add( UNSET_PARAMETER );
					types.Add( UNSET_TYPE );
				}
				values.Add( val );
				types.Add( type );
			}
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public IQuery SetParameter( string name, object val, IType type )
		{
			namedParameters[ name ] = new TypedValue( type, val );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetAnsiString( int position, string val )
		{
			SetParameter( position, val, NHibernateUtil.AnsiString );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetString( int position, string val )
		{
			SetParameter( position, val, NHibernateUtil.String );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetCharacter( int position, char val )
		{
			SetParameter( position, val, NHibernateUtil.Character ); // );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetBoolean( int position, bool val )
		{
			SetParameter( position, val, NHibernateUtil.Boolean ); // );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetByte( int position, byte val )
		{
			SetParameter( position, val, NHibernateUtil.Byte );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetInt16( int position, short val )
		{
			SetParameter( position, val, NHibernateUtil.Int16 );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetInt32( int position, int val )
		{
			SetParameter( position, val, NHibernateUtil.Int32 );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetInt64( int position, long val )
		{
			SetParameter( position, val, NHibernateUtil.Int64 );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetSingle( int position, float val )
		{
			SetParameter( position, val, NHibernateUtil.Single );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetDouble( int position, double val )
		{
			SetParameter( position, val, NHibernateUtil.Double );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetBinary( int position, byte[ ] val )
		{
			SetParameter( position, val, NHibernateUtil.Binary );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetDecimal( int position, decimal val )
		{
			SetParameter( position, val, NHibernateUtil.Decimal );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetDateTime( int position, DateTime val )
		{
			SetParameter( position, val, NHibernateUtil.DateTime );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetTime( int position, DateTime val )
		{
			SetParameter( position, val, NHibernateUtil.Time );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetTimestamp( int position, DateTime val )
		{
			SetParameter( position, val, NHibernateUtil.Timestamp );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetEntity( int position, object val )
		{
			SetParameter( position, val, NHibernateUtil.Entity( NHibernateProxyHelper.GetClass( val ) ) );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetEnum( int position, Enum val )
		{
			SetParameter( position, val, NHibernateUtil.Enum( val.GetType() ) );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetAnsiString( string name, string val )
		{
			SetParameter( name, val, NHibernateUtil.AnsiString );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetString( string name, string val )
		{
			SetParameter( name, val, NHibernateUtil.String );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetCharacter( string name, char val )
		{
			SetParameter( name, val, NHibernateUtil.Character );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetBoolean( string name, bool val )
		{
			SetParameter( name, val, NHibernateUtil.Boolean );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetByte( string name, byte val )
		{
			SetParameter( name, val, NHibernateUtil.Byte );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetInt16( string name, short val )
		{
			SetParameter( name, val, NHibernateUtil.Int16 );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetInt32( string name, int val )
		{
			SetParameter( name, val, NHibernateUtil.Int32 );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetInt64( string name, long val )
		{
			SetParameter( name, val, NHibernateUtil.Int64 );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetSingle( string name, float val )
		{
			SetParameter( name, val, NHibernateUtil.Single );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetDouble( string name, double val )
		{
			SetParameter( name, val, NHibernateUtil.Double );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetBinary( string name, byte[ ] val )
		{
			SetParameter( name, val, NHibernateUtil.Binary );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetDecimal( string name, decimal val )
		{
			SetParameter( name, val, NHibernateUtil.Decimal );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetDateTime( string name, DateTime val )
		{
			SetParameter( name, val, NHibernateUtil.DateTime );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetTime( string name, DateTime val )
		{
			SetParameter( name, val, NHibernateUtil.Time ); 
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetTimestamp( string name, DateTime val )
		{
			SetParameter( name, val, NHibernateUtil.Timestamp );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetEntity( string name, object val )
		{
			SetParameter( name, val, NHibernateUtil.Entity( NHibernateProxyHelper.GetClass( val ) ) );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetEnum( string name, Enum val )
		{
			SetParameter( name, val, NHibernateUtil.Enum( val.GetType() ) );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetParameter( string name, object val )
		{
			if( val==null )
			{
				throw new ArgumentNullException( "val", "A type specific Set(name, val) should be called because the Type can not be guessed from a null value." );
			}
			SetParameter( name, val, GuessType( val ) );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public IQuery SetParameter( int position, object val )
		{
			if( val==null )
			{
				throw new ArgumentNullException( "val", "A type specific Set(position, val) should be called because the Type can not be guessed from a null value." );
			}
			SetParameter( position, val, GuessType( val ) );
			return this;
		}

		/// <summary>
		/// Guesses the <see cref="IType"/> from the <c>param</c>'s value.
		/// </summary>
		/// <param name="param">The object to guess the <see cref="IType"/> of.</param>
		/// <returns>An <see cref="IType"/> for the object.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the <c>param</c> is null because the <see cref="IType"/>
		/// can't be guess from a null value.
		/// </exception>
		private IType GuessType( object param )
		{
			if( param==null )
			{
				throw new ArgumentNullException( "param", "The IType can not be guessed for a null value." );
			}
			
			System.Type clazz = NHibernateProxyHelper.GetClass( param );
			return GuessType( clazz );
		}

		/// <summary>
		/// Guesses the <see cref="IType"/> from the <see cref="System.Type"/>.
		/// </summary>
		/// <param name="clazz">The <see cref="System.Type"/> to guess the <see cref="IType"/> of.</param>
		/// <returns>An <see cref="IType"/> for the <see cref="System.Type"/>.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the <c>clazz</c> is null because the <see cref="IType"/>
		/// can't be guess from a null type.
		/// </exception>
		private IType GuessType( System.Type clazz )
		{
			if( clazz==null )
			{
				throw new ArgumentNullException( "clazz", "The IType can not be guessed for a null value." );
			}
			
			string typename = clazz.AssemblyQualifiedName;
			IType type = TypeFactory.HeuristicType( typename );
			bool serializable = (type!=null && type is SerializableType);
			if( type==null || serializable )
			{
				try
				{
					session.Factory.GetEntityPersister( clazz );
				}
				catch( MappingException )
				{
					if( serializable )
					{
						return type;
					}
					else
					{
						throw new HibernateException( "Could not determine a type for class: " + typename );
					}
				}
				return NHibernateUtil.Entity( clazz );
			}
			else
			{
				return type;
			}
		}

		/// <summary></summary>
		public virtual IType[ ] ReturnTypes
		{
			get { return session.Factory.GetReturnTypes( queryString ); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="vals"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public IQuery SetParameterList( string name, ICollection vals, IType type )
		{
			if ( !actualNamedParameters.Contains(name) )
				throw new ArgumentOutOfRangeException(string.Format("Parameter {0} does not exist as a named parameter in [{1}]", name, queryString));

			namedParameterLists.Add( name, new TypedValue( type, vals ) );
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="namedParams"></param>
		/// <returns></returns>
		protected string BindParameterLists( IDictionary namedParams )
		{
			string query = queryString;
			foreach( DictionaryEntry de in namedParameterLists )
			{
				query = BindParameterList( query, ( string ) de.Key, ( TypedValue ) de.Value, namedParams );
			}
			return query;
		}

		private string BindParameterList( string queryString, string name, TypedValue typedList, IDictionary namedParams )
		{
			ICollection vals = ( ICollection ) typedList.Value;
			IType type = typedList.Type;
			StringBuilder list = new StringBuilder( 16 );
			int i = 0;
			foreach( object obj in vals )
			{
				string alias = name + i++ + StringHelper.Underscore;
				namedParams.Add( alias, new TypedValue( type, obj ) );
				list.Append( ParserHelper.HqlVariablePrefix + alias );
				if( i < vals.Count )
				{
					list.Append( StringHelper.CommaSpace );
				}
			}

			return StringHelper.Replace( queryString, ParserHelper.HqlVariablePrefix + name, list.ToString() );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="vals"></param>
		/// <returns></returns>
		public IQuery SetParameterList( string name, ICollection vals )
		{
			foreach( object obj in vals )
			{
				SetParameterList( name, vals, GuessType( obj ) );
				break; // fairly hackish...need the type of the first object
			}
			return this;
		}

		private void InitParameterBookKeeping()
		{
			StringTokenizer st = new StringTokenizer( queryString, ParserHelper.HqlSeparators, true );
			ISet result = new HashedSet();

			IEnumerator enumer = st.GetEnumerator();
			while( enumer.MoveNext() ) 
			{
				string str = (string) enumer.Current;
				if ( str.StartsWith( ParserHelper.HqlVariablePrefix ) )
				{
					result.Add( str.Substring( 1 ) );
				}
			}

			actualNamedParameters = result;
			// TODO: This is weak as it doesn't take account of ? embedded in the SQL
			positionalParameterCount = StringHelper.CountUnquoted( queryString, StringHelper.SqlParameter.ToCharArray()[0] );
		}

		/// <summary></summary>
		public string[ ] NamedParameters
		{
			get
			{
				string[ ] retVal = new String[actualNamedParameters.Count];
				int i = 0;
				foreach( string parm in actualNamedParameters )
				{
					retVal[ i++ ] = parm;
				}
				return retVal;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="bean"></param>
		/// <returns></returns>
		public IQuery SetProperties( object bean )
		{
			System.Type clazz = bean.GetType();
			foreach( string namedParam in actualNamedParameters )
			{
				try
				{
					IGetter getter = ReflectHelper.GetGetter( clazz, namedParam, "property" );
					SetParameter( namedParam, getter.Get( bean ), GuessType( getter.ReturnType ) );
				}
				catch( Exception )
				{
				}
			}
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="vals"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public IQuery SetParameterList( string name, object[ ] vals, IType type )
		{
			return SetParameterList( name, vals, type );
		}

		//
		public IQuery SetParameterList( string name, object[ ] vals )
		{
			return SetParameterList( name, vals );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="lockMode"></param>
		public virtual void SetLockMode( string alias, LockMode lockMode )
		{
			lockModes[ alias ] = lockMode;
		}

		/// <summary></summary>
		public IDictionary LockModes
		{
			get { return lockModes; }
		}

		/// <summary></summary>
		internal ISessionImplementor Session
		{
			get { return session; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public object UniqueResult()
		{
			return UniqueElement( List() );
		}

		internal static object UniqueElement( IList list )
		{
			int size = list.Count;
			if ( size == 0 )
			{
				return null;
			}
			object first = list[ 0 ];
			for ( int i = 1; i < size; i++ )
			{
				if ( list[ i ] != first )
				{
					throw new NonUniqueResultException( size );
				}
			}
			return first;
		}

		protected RowSelection RowSelection
		{
			get { return selection; }
		}

		public virtual IType[] TypeArray()
		{
			return ( IType[ ] ) types.ToArray( typeof( IType ) );
		}

		public virtual object[] ValueArray()
		{
			return ( object[ ] ) values.ToArray( typeof( object ) );
		}

		public QueryParameters GetQueryParameters( IDictionary namedParams )
		{
			return new QueryParameters(
				TypeArray(),
				ValueArray(),
				namedParams,
				lockModes,
				selection,
				cacheable,
				cacheRegion,
				forceCacheRefresh,
				null);
		}

		public IQuery SetCacheable( bool cacheable )
		{
			this.cacheable = cacheable;
			return this;
		}

		public IQuery SetCacheRegion( string cacheRegion )
		{
			if( cacheRegion != null )
				this.cacheRegion = cacheRegion.Trim();
			return this;
		}

		public IQuery SetForceCacheRefresh( bool forceCacheRefresh )
		{
			this.forceCacheRefresh = forceCacheRefresh;
			return this;
		}

		// Remaining methods of IQuery interface - left for children to implement
		public abstract IEnumerable Enumerable();
		public abstract IEnumerable<T> Enumerable<T>();
		public abstract IList List();
		public abstract IList<T> List<T>();
	}
}

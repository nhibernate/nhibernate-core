using System;
using System.Collections;
using System.Text;
using NHibernate.Engine;
using NHibernate.Property;
using NHibernate.Proxy;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Impl
{
	internal class QueryImpl : IQuery
	{
		private ISessionImplementor session;
		private string queryString;

		private RowSelection selection;
		private ArrayList values = new ArrayList( 4 );
		private ArrayList types = new ArrayList( 4 );
		private IDictionary namedParameters = new Hashtable( 4 );
		private IDictionary namedParametersLists = new Hashtable( 4 );
		private IDictionary lockModes = new Hashtable( 2 );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="queryString"></param>
		/// <param name="session"></param>
		public QueryImpl( string queryString, ISessionImplementor session )
		{
			this.session = session;
			this.queryString = queryString;
			selection = new RowSelection();
		}

		/// <summary></summary>
		public virtual IEnumerable Enumerable()
		{
			//TODO: see if there is a better way to implement
			QueryParameters qp = new QueryParameters(
				( IType[ ] ) types.ToArray( typeof( IType ) ),
				( object[ ] ) values.ToArray( typeof( object ) ),
				new Hashtable( namedParameters ),
				lockModes,
				selection );

			string query = BindParameterLists( qp.NamedParameters );
			return session.Enumerable( query, qp );
		}

		/// <summary></summary>
		public IDictionary LockModes
		{
			get { return lockModes; }
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
		public virtual IList List()
		{
			//TODO: see if there is a better way to implement
			QueryParameters qp = new QueryParameters(
				( IType[ ] ) types.ToArray( typeof( IType ) ),
				( object[ ] ) values.ToArray( typeof( object ) ),
				new Hashtable( namedParameters ),
				lockModes,
				selection );

			string query = BindParameterLists( qp.NamedParameters );
			return session.Find( query, qp );
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
			int size = values.Count;
			if( position < size )
			{
				values[ position ] = val;
				types[ position ] = type;
			}
			else
			{
				for( int i = 0; i < position - size; i++ )
				{
					values.Add( null );
					types.Add( null );
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
			SetParameter( position, val, NHibernateUtil.DateTime ); //TODO: change to time
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
			SetParameter( name, val, NHibernateUtil.DateTime ); //TODO: change to time
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
			SetParameter( position, val, GuessType( val ) );
			return this;
		}

		private IType GuessType( object param )
		{
			System.Type clazz = NHibernateProxyHelper.GetClass( param );
			return GuessType( clazz );
		}

		private IType GuessType( System.Type clazz )
		{
			string typename = clazz.AssemblyQualifiedName;
			IType type = TypeFactory.HueristicType( typename );
			bool serializable = type != null && type is SerializableType;
			if( type == null || serializable )
			{
				try
				{
					session.Factory.GetPersister( clazz );
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
		public IType[ ] ReturnTypes
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
			namedParametersLists.Add( name, new TypedValue( type, vals ) );
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
			foreach( DictionaryEntry de in namedParametersLists )
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
				list.Append( ':' + alias );
				if( i < vals.Count )
				{
					list.Append( StringHelper.CommaSpace );
				}
			}

			return StringHelper.Replace( queryString, ':' + name, list.ToString() );
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

		/// <summary></summary>
		public string[ ] NamedParameters
		{
			get
			{
				ICollection parms = session.Factory.GetNamedParameters( queryString );
				string[ ] retVal = new String[parms.Count];
				int i = 0;
				foreach( string parm in parms )
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
			foreach( string namedParam in session.Factory.GetNamedParameters( queryString ) )
			{
				try
				{
					IGetter getter = ReflectHelper.GetGetter( clazz, namedParam );
					SetParameter( namedParam, getter.Get( bean ), GuessType( getter.ReturnType ) );
				}
				catch( Exception )
				{
				}
			}
			return this;
		}

		/// <summary></summary>
		internal ISessionImplementor Session
		{
			get { return session; }
		}

		/// <summary></summary>
		internal ArrayList Values
		{
			get { return values; }
		}

		/// <summary></summary>
		internal ArrayList Types
		{
			get { return types; }
		}

		internal RowSelection Selection
		{
			get { return selection; }
		}

		/// <summary></summary>
		public string QueryString
		{
			get { return queryString; }
		}

		/// <summary></summary>
		internal IDictionary NamedParams
		{
			get { return namedParameters; }
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

	}
}
using System;
using System.Collections;
using System.Text;

using NHibernate.Engine;
using NHibernate.Proxy;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Impl {
	
	internal class QueryImpl : IQuery {
		private ISessionImplementor session;
		private string queryString;

		private RowSelection selection;
		private ArrayList values = new ArrayList(4);
		private ArrayList types = new ArrayList(4);
		private IDictionary namedParameters = new Hashtable(4);
		private IDictionary namedParametersLists = new Hashtable(4);
		private IDictionary lockModes = new Hashtable(2);

		public QueryImpl(string queryString, ISessionImplementor session) {
			this.session = session;
			this.queryString = queryString;
			selection = new RowSelection();
		}

		public virtual IEnumerable Enumerable() 
		{
			//TODO: see if there is a better way to implement
			QueryParameters qp = new QueryParameters( 
				(IType[])types.ToArray( typeof(IType) ),
				(object[])values.ToArray( typeof(object) ), 
				new Hashtable( namedParameters ),
				lockModes,
				selection );

			string query = BindParameterLists( qp.NamedParameters );
			return session.Enumerable( query, qp );
		}

	    public IDictionary LockModes
	    {
	        get { return lockModes; }
	    }

		public virtual void SetLockMode(string alias, LockMode lockMode)
		{
			lockModes[alias] = lockMode;    
		}

	    public virtual IList List() 
		{
			//TODO: see if there is a better way to implement
			QueryParameters qp = new QueryParameters( 
				(IType[])types.ToArray( typeof(IType) ),
				(object[])values.ToArray( typeof(object) ), 
				new Hashtable( namedParameters ),
				lockModes,
				selection );

			string query = BindParameterLists( qp.NamedParameters );
			return session.Find( query, qp );
		}

		public IQuery SetMaxResults(int maxResults) {
			selection.MaxRows = maxResults;
			return this;
		}

		public IQuery SetTimeout(int timeout) {
			selection.Timeout = timeout;
			return this;
		}

		public IQuery SetFirstResult(int firstResult) {
			selection.FirstRow = firstResult;
			return this;
		}

		public IQuery SetParameter(int position, object val, IType type) {
			int size = values.Count;
			if ( position<size ) {
				values[position] = val;
				types[position] = type;
			} else {
				for (int i=0; i<position-size; i++) {
					values.Add(null);
					types.Add(null);
				}
				values.Add(val);
				types.Add(type);
			}
			return this;
		}

		public IQuery SetParameter(string name, object val, IType type) {
			namedParameters[name] = new TypedValue(type, val);
			return this;
		}

		public IQuery SetAnsiString(int position, string val) 
		{
			SetParameter(position, val, NHibernate.AnsiString);
			return this;
		}

		public IQuery SetString(int position, string val) {
			SetParameter(position, val, NHibernate.String);
			return this;
		}
		public IQuery SetCharacter(int position, char val) {
			SetParameter(position, val, NHibernate.Character);// );
			return this;
		}
		public IQuery SetBoolean(int position, bool val) {
			SetParameter(position, val, NHibernate.Boolean); // );
			return this;
		}
		public IQuery SetByte(int position, byte val) {
			SetParameter(position, val, NHibernate.Byte);
			return this;
		}
		public IQuery SetInt16(int position, short val) {
			SetParameter(position, val, NHibernate.Int16);
			return this;
		}
		public IQuery SetInt32(int position, int val) {
			SetParameter(position, val, NHibernate.Int32);
			return this;
		}
		public IQuery SetInt64(int position, long val) {
			SetParameter(position, val, NHibernate.Int64);
			return this;
		}
		public IQuery SetSingle(int position, float val) {
			SetParameter(position, val, NHibernate.Single);
			return this;
		}
		public IQuery SetDouble(int position, double val) {
			SetParameter(position, val, NHibernate.Double);
			return this;
		}
		public IQuery SetBinary(int position, byte[] val) {
			SetParameter(position, val, NHibernate.Binary);
			return this;
		}
		public IQuery SetDecimal(int position, decimal val) {
			SetParameter(position, val, NHibernate.Decimal);
			return this;
		}
		public IQuery SetDateTime(int position, DateTime val) {
			SetParameter(position, val, NHibernate.DateTime);
			return this;
		}
		public IQuery SetTime(int position, DateTime val) {
			SetParameter(position, val, NHibernate.DateTime); //TODO: change to time
			return this;
		}
		public IQuery SetTimestamp(int position, DateTime val) {
			SetParameter(position, val, NHibernate.Timestamp);
			return this;
		}

		public IQuery SetEntity(int position, object val) 
		{
			SetParameter( position, val, NHibernate.Entity( NHibernateProxyHelper.GetClass( val ) ) );
			return this;
		}

		public IQuery SetEnum(int position, System.Enum val) {
			SetParameter(position, val, NHibernate.Enum( val.GetType() ) );
			return this;
		}

		public IQuery SetAnsiString(string name, string val) 
		{
			SetParameter(name, val, NHibernate.AnsiString);
			return this;
		}

		public IQuery SetString(string name, string val) {
			SetParameter(name, val, NHibernate.String);
			return this;
		}
		public IQuery SetCharacter(string name, char val) {
			SetParameter(name, val, NHibernate.Character);
			return this;
		}
		public IQuery SetBoolean(string name, bool val) {
			SetParameter(name, val, NHibernate.Boolean);
			return this;
		}
		public IQuery SetByte(string name, byte val) {
			SetParameter(name, val, NHibernate.Byte);
			return this;
		}
		public IQuery SetInt16(string name, short val) {
			SetParameter(name, val, NHibernate.Int16);
			return this;
		}
		public IQuery SetInt32(string name, int val) {
			SetParameter(name, val, NHibernate.Int32);
			return this;
		}
		public IQuery SetInt64(string name, long val) {
			SetParameter(name, val, NHibernate.Int64);
			return this;
		}
		
		public IQuery SetSingle(string name, float val) {
			SetParameter(name, val, NHibernate.Single);
			return this;
		}
		public IQuery SetDouble(string name, double val) {
			SetParameter(name, val, NHibernate.Double);
			return this;
		}
		public IQuery SetBinary(string name, byte[] val) {
			SetParameter(name, val, NHibernate.Binary);
			return this;
		}
		public IQuery SetDecimal(string name, decimal val) {
			SetParameter(name, val, NHibernate.Decimal);
			return this;
		}
		public IQuery SetDateTime(string name, DateTime val) {
			SetParameter(name, val, NHibernate.DateTime);
			return this;
		}
		public IQuery SetTime(string name, DateTime val) {
			SetParameter(name, val, NHibernate.DateTime); //TODO: change to time
			return this;
		}
		public IQuery SetTimestamp(string name, DateTime val) {
			SetParameter(name, val, NHibernate.Timestamp);
			return this;
		}
		public IQuery SetEntity(string name, object val) {
			SetParameter(name, val, NHibernate.Entity( NHibernateProxyHelper.GetClass( val ) ) );
			return this;
		}
		public IQuery SetEnum(string name, System.Enum val) {
			SetParameter(name, val, NHibernate.Enum( val.GetType() ) );
			return this;
		}
		

		public IQuery SetParameter(string name, object val) {
			SetParameter(name, val, GuessType(val.GetType()) );
			return this;
		}
		public IQuery SetParameter(int position, object val) {
			SetParameter(position, val, GuessType(val.GetType()) );
			return this;
		}
		
		private IType GuessType(object param) {
			System.Type clazz = NHibernateProxyHelper.GetClass(param); 
			return GuessType(clazz);
		}

		private IType GuessType(System.Type clazz) 
		{
			string typename = clazz.AssemblyQualifiedName;
			IType type = TypeFactory.HueristicType( typename ); 
			bool serializable = type!=null && type is SerializableType;
			if ( type==null || serializable ) 
			{
				try 
				{
					session.Factory.GetPersister(clazz);
				} 
				catch (MappingException) 
				{
					if (serializable) 
					{ 
						return type; 
					} 
					else 
					{ 
						throw new HibernateException("Could not determine a type for class: " + typename);
					}
				}
				return NHibernate.Entity(clazz);
			}
			else 
			{
				return type;
			}
		}

		public IType[] ReturnTypes {
			get { return session.Factory.GetReturnTypes(queryString); }
		}

		public IQuery SetParameterList(string name, ICollection vals, IType type) {
			namedParametersLists.Add( name, new TypedValue( type, vals ) );
			return this;
		}

		protected string BindParameterLists(IDictionary namedParams) {
			string query = queryString;
			foreach( DictionaryEntry de in namedParametersLists ) {
				query = BindParameterList( query, (string) de.Key, (TypedValue) de.Value, namedParams );
			}
			return query;
		}

		private string BindParameterList(string queryString, string name, TypedValue typedList, IDictionary namedParams) {
			ICollection vals = (ICollection) typedList.Value;
			IType type = typedList.Type;
			StringBuilder list = new StringBuilder(16);
			int i=0;
			foreach( object obj in vals ) {
				string alias = name + i++ + StringHelper.Underscore;
				namedParams.Add( alias, new TypedValue( type, obj ) );
				list.Append( ':' + alias );
				if ( i < vals.Count ) list.Append( StringHelper.CommaSpace );
			}

			return StringHelper.Replace( queryString, ':' + name, list.ToString() );
		}

		public IQuery SetParameterList(string name, ICollection vals) {
			foreach(object obj in vals) 
			{
				SetParameterList(name, vals, GuessType(obj) );
				break; // fairly hackish...need the type of the first object
			}
			return this;
		}

		public string[] NamedParameters {
			get {
				ICollection parms = session.Factory.GetNamedParameters(queryString);
				string[] retVal = new String[parms.Count];
				int i=0;
				foreach(string parm in parms) retVal[i++] = parm;
				return retVal;
			}
		}

		public IQuery SetProperties(object bean) {
			System.Type clazz = bean.GetType();
			foreach(string namedParam in session.Factory.GetNamedParameters(queryString)) {
				try {
					Property.IGetter getter = ReflectHelper.GetGetter(clazz, namedParam);
					SetParameter( namedParam, getter.Get(bean), GuessType( getter.ReturnType ) );
				} catch (Exception) {}
			}
			return this;
		}

		internal ISessionImplementor Session {
			get { return session; }
		}

		internal ArrayList Values {
			get { return values; }
		}

		internal ArrayList Types {
			get { return types; }
		}

		internal RowSelection Selection {
			get { return selection; }
		}
		public string QueryString {
			get { return queryString; }
		}

		internal IDictionary NamedParams {
			get { return namedParameters; }
		}

		public IQuery SetParameterList(string name, object[] vals, IType type) { 
			return SetParameterList(name, vals, type); 
		} 
    
		public IQuery SetParameterList(string name, object[] vals) { 
			return SetParameterList( name, vals ); 
		} 

	}
}

using System;
using System.Collections;
using System.Text;
using Iesi.Collections;
using NHibernate.Engine;
using NHibernate.Hql.Classic;
using NHibernate.Property;
using NHibernate.Proxy;
using NHibernate.Transform;
using NHibernate.Type;
using NHibernate.Util;
using System.Collections.Generic;

namespace NHibernate.Impl
{
	/// <summary>
	/// Abstract implementation of the IQuery interface.
	/// </summary>
	public abstract class AbstractQueryImpl : IQuery
	{
		private string queryString;
		private IDictionary lockModes = new Hashtable(2);

		private readonly ISessionImplementor session;

		private RowSelection selection;
		private ArrayList values = new ArrayList(4);
		private ArrayList types = new ArrayList(4);
		private int positionalParameterCount = 0;
		private ISet actualNamedParameters;
		private IDictionary namedParameters = new Hashtable(4);
		private IDictionary namedParameterLists = new Hashtable(4);
		private bool cacheable;
		private string cacheRegion;
		private bool readOnly;
		private static readonly object UNSET_PARAMETER = new object();
		private static readonly object UNSET_TYPE = new object();
		private object optionalId;
		private object optionalObject;
		private System.Type optionalEntityName;
		private FlushMode flushMode = FlushMode.Unspecified;
		private FlushMode sessionFlushMode = FlushMode.Unspecified;
		private object collectionKey;
		private IResultTransformer resultTransformer;
		private bool shouldIgnoredUnknownNamedParameters;
		private CacheMode? cacheMode;
		private CacheMode? sessionCacheMode;

		public AbstractQueryImpl(string queryString, FlushMode flushMode, ISessionImplementor session)
		{
			this.session = session;
			this.queryString = queryString;
			this.flushMode = flushMode;
			selection = new RowSelection();
			cacheMode = null;
			InitParameterBookKeeping();
		}

		public string QueryString
		{
			get { return queryString; }
		}

		protected internal IDictionary NamedParams
		{
			// NB The java one always returns a copy, so I'm going to reproduce that behaviour
			get { return new Hashtable(namedParameters); }
		}

		public bool HasNamedParameters
		{
			get { return actualNamedParameters.Count > 0; }
		}

		protected internal virtual void VerifyParameters()
		{
			if (actualNamedParameters.Count > namedParameters.Count + namedParameterLists.Count)
			{
				Set missingParams = new ListSet(actualNamedParameters);
				missingParams.RemoveAll(namedParameterLists.Keys);
				missingParams.RemoveAll(namedParameters.Keys);
				throw new QueryException("Not all named parameters have been set: " + CollectionPrinter.ToString(missingParams),
																 QueryString);
			}

			int positionalValueSpan = 0;
			for (int i = 0; i < values.Count; i++)
			{
				object obj = types[i];
				if (values[i] == UNSET_PARAMETER || obj == UNSET_TYPE)
				{
					throw new QueryException("Unset positional parameter at position: " + i, QueryString);
				}
				positionalValueSpan += ((IType)obj).GetColumnSpan(session.Factory);
			}

			if (positionalParameterCount != positionalValueSpan)
			{
				throw new QueryException(
						string.Format("Not all positional parameters have been set. Expected {0}, set {1}", positionalParameterCount,
													values.Count),
						QueryString);
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

		public IQuery SetMaxResults(int maxResults)
		{
			selection.MaxRows = maxResults;
			return this;
		}

		public IQuery SetTimeout(int timeout)
		{
			selection.Timeout = timeout;
			return this;
		}

		public IQuery SetFetchSize(int fetchSize)
		{
			selection.FetchSize = fetchSize;
			return this;
		}

		public IQuery SetFirstResult(int firstResult)
		{
			selection.FirstRow = firstResult;
			return this;
		}

		public IQuery SetParameter(int position, object val, IType type)
		{
			if (positionalParameterCount == 0)
			{
				throw new ArgumentOutOfRangeException(string.Format("No positional parameters in query: {0}", QueryString));
			}

			if (position < 0 || position > positionalParameterCount - 1)
			{
				throw new ArgumentOutOfRangeException(
						string.Format("Positional parameter does not exists: {0} in query: {1}", position, QueryString));
			}

			int size = values.Count;
			if (position < size)
			{
				values[position] = val;
				types[position] = type;
			}
			else
			{
				// Put guard values in for any positions before the wanted position - allows us to detect unset parameters on validate.
				for (int i = 0; i < position - size; i++)
				{
					values.Add(UNSET_PARAMETER);
					types.Add(UNSET_TYPE);
				}
				values.Add(val);
				types.Add(type);
			}
			return this;
		}

		public IQuery SetParameter(string name, object val, IType type)
		{
			namedParameters[name] = new TypedValue(type, val);
			return this;
		}

		public IQuery SetAnsiString(int position, string val)
		{
			SetParameter(position, val, NHibernateUtil.AnsiString);
			return this;
		}

		public IQuery SetString(int position, string val)
		{
			SetParameter(position, val, NHibernateUtil.String);
			return this;
		}

		public IQuery SetCharacter(int position, char val)
		{
			SetParameter(position, val, NHibernateUtil.Character); // );
			return this;
		}

		public IQuery SetBoolean(int position, bool val)
		{
			SetParameter(position, val, NHibernateUtil.Boolean); // );
			return this;
		}

		public IQuery SetByte(int position, byte val)
		{
			SetParameter(position, val, NHibernateUtil.Byte);
			return this;
		}

		public IQuery SetInt16(int position, short val)
		{
			SetParameter(position, val, NHibernateUtil.Int16);
			return this;
		}

		public IQuery SetInt32(int position, int val)
		{
			SetParameter(position, val, NHibernateUtil.Int32);
			return this;
		}

		public IQuery SetInt64(int position, long val)
		{
			SetParameter(position, val, NHibernateUtil.Int64);
			return this;
		}

		public IQuery SetSingle(int position, float val)
		{
			SetParameter(position, val, NHibernateUtil.Single);
			return this;
		}

		public IQuery SetDouble(int position, double val)
		{
			SetParameter(position, val, NHibernateUtil.Double);
			return this;
		}

		public IQuery SetBinary(int position, byte[] val)
		{
			SetParameter(position, val, NHibernateUtil.Binary);
			return this;
		}

		public IQuery SetDecimal(int position, decimal val)
		{
			SetParameter(position, val, NHibernateUtil.Decimal);
			return this;
		}

		public IQuery SetDateTime(int position, DateTime val)
		{
			SetParameter(position, val, NHibernateUtil.DateTime);
			return this;
		}

		public IQuery SetTime(int position, DateTime val)
		{
			SetParameter(position, val, NHibernateUtil.Time);
			return this;
		}

		public IQuery SetTimestamp(int position, DateTime val)
		{
			SetParameter(position, val, NHibernateUtil.Timestamp);
			return this;
		}

		public IQuery SetEntity(int position, object val)
		{
			SetParameter(position, val, NHibernateUtil.Entity(NHibernateProxyHelper.GuessClass(val)));
			return this;
		}

		public IQuery SetEnum(int position, Enum val)
		{
			SetParameter(position, val, NHibernateUtil.Enum(val.GetType()));
			return this;
		}

		public IQuery SetAnsiString(string name, string val)
		{
			SetParameter(name, val, NHibernateUtil.AnsiString);
			return this;
		}

		public IQuery SetString(string name, string val)
		{
			SetParameter(name, val, NHibernateUtil.String);
			return this;
		}

		public IQuery SetCharacter(string name, char val)
		{
			SetParameter(name, val, NHibernateUtil.Character);
			return this;
		}

		public IQuery SetBoolean(string name, bool val)
		{
			SetParameter(name, val, NHibernateUtil.Boolean);
			return this;
		}

		public IQuery SetByte(string name, byte val)
		{
			SetParameter(name, val, NHibernateUtil.Byte);
			return this;
		}

		public IQuery SetInt16(string name, short val)
		{
			SetParameter(name, val, NHibernateUtil.Int16);
			return this;
		}

		public IQuery SetInt32(string name, int val)
		{
			SetParameter(name, val, NHibernateUtil.Int32);
			return this;
		}

		public IQuery SetInt64(string name, long val)
		{
			SetParameter(name, val, NHibernateUtil.Int64);
			return this;
		}

		public IQuery SetSingle(string name, float val)
		{
			SetParameter(name, val, NHibernateUtil.Single);
			return this;
		}

		public IQuery SetDouble(string name, double val)
		{
			SetParameter(name, val, NHibernateUtil.Double);
			return this;
		}

		public IQuery SetBinary(string name, byte[] val)
		{
			SetParameter(name, val, NHibernateUtil.Binary);
			return this;
		}

		public IQuery SetDecimal(string name, decimal val)
		{
			SetParameter(name, val, NHibernateUtil.Decimal);
			return this;
		}

		public IQuery SetDateTime(string name, DateTime val)
		{
			SetParameter(name, val, NHibernateUtil.DateTime);
			return this;
		}

		public IQuery SetTime(string name, DateTime val)
		{
			SetParameter(name, val, NHibernateUtil.Time);
			return this;
		}

		public IQuery SetTimestamp(string name, DateTime val)
		{
			SetParameter(name, val, NHibernateUtil.Timestamp);
			return this;
		}

		public IQuery SetGuid(string name, Guid val)
		{
			SetParameter(name, val, NHibernateUtil.Guid);
			return this;
		}

		public IQuery SetGuid(int position, Guid val)
		{
			SetParameter(position, val, NHibernateUtil.Guid);
			return this;
		}

		public IQuery SetEntity(string name, object val)
		{
			SetParameter(name, val, NHibernateUtil.Entity(NHibernateProxyHelper.GuessClass(val)));
			return this;
		}

		public IQuery SetEnum(string name, Enum val)
		{
			SetParameter(name, val, NHibernateUtil.Enum(val.GetType()));
			return this;
		}

		public IQuery SetParameter(string name, object val)
		{
			if (val == null)
			{
				throw new ArgumentNullException("val",
																				"A type specific Set(name, val) should be called because the Type can not be guessed from a null value.");
			}
			SetParameter(name, val, GuessType(val));
			return this;
		}

		public IQuery SetParameter(int position, object val)
		{
			if (val == null)
			{
				throw new ArgumentNullException("val",
																				"A type specific Set(position, val) should be called because the Type can not be guessed from a null value.");
			}
			SetParameter(position, val, GuessType(val));
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
		private IType GuessType(object param)
		{
			if (param == null)
			{
				throw new ArgumentNullException("param", "The IType can not be guessed for a null value.");
			}

			System.Type clazz = NHibernateProxyHelper.GuessClass(param);
			return GuessType(clazz);
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
		private IType GuessType(System.Type clazz)
		{
			if (clazz == null)
			{
				throw new ArgumentNullException("clazz", "The IType can not be guessed for a null value.");
			}

			string typename = clazz.AssemblyQualifiedName;
			IType type = TypeFactory.HeuristicType(typename);
			bool serializable = (type != null && type is SerializableType);
			if (type == null || serializable)
			{
				try
				{
					session.Factory.GetEntityPersister(clazz);
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
				return NHibernateUtil.Entity(clazz);
			}
			else
			{
				return type;
			}
		}

		public virtual IType[] ReturnTypes
		{
			get { return session.Factory.GetReturnTypes(queryString); }
		}

		public IQuery SetParameterList(string name, IEnumerable vals, IType type)
		{
			if (!actualNamedParameters.Contains(name))
			{
				if (shouldIgnoredUnknownNamedParameters)//just ignore it
					return this;
				throw new ArgumentOutOfRangeException(
						string.Format("Parameter {0} does not exist as a named parameter in [{1}]", name, queryString));
			}

			namedParameterLists.Add(name, new TypedValue(type, vals));
			return this;
		}

		public string BindParameterLists()
		{
			return BindParameterLists(NamedParams);
		}

		protected internal string BindParameterLists(IDictionary namedParams)
		{
			string query = queryString;
			foreach (DictionaryEntry de in namedParameterLists)
			{
				query = BindParameterList(query, (string)de.Key, (TypedValue)de.Value, namedParams);
			}
			return query;
		}

		private string BindParameterList(string queryString, string name, TypedValue typedList, IDictionary namedParams)
		{
			IEnumerable vals = (IEnumerable)typedList.Value;
			IType type = typedList.Type;
			StringBuilder list = new StringBuilder(16);
			int i = 0;
			foreach (object obj in vals)
			{
				if (i > 0)
				{
					list.Append(StringHelper.CommaSpace);
				}
				string alias = name + i++ + StringHelper.Underscore;
				namedParams.Add(alias, new TypedValue(type, obj));
				list.Append(ParserHelper.HqlVariablePrefix + alias);
			}

			return StringHelper.Replace(queryString, ParserHelper.HqlVariablePrefix + name, list.ToString());
		}

		public IQuery SetParameterList(string name, IEnumerable vals)
		{
			foreach (object obj in vals)
			{
				SetParameterList(name, vals, GuessType(obj));
				break; // fairly hackish...need the type of the first object
			}
			return this;
		}

		private void InitParameterBookKeeping()
		{
			StringTokenizer st = new StringTokenizer(queryString, ParserHelper.HqlSeparators, true);
			ISet result = new HashedSet();

			foreach (string str in st)
			{
				if (str.StartsWith(ParserHelper.HqlVariablePrefix))
				{
					result.Add(str.Substring(1));
				}
			}

			actualNamedParameters = result;
			// TODO: This is weak as it doesn't take account of ? embedded in the SQL
			positionalParameterCount = StringHelper.CountUnquoted(queryString, StringHelper.SqlParameter.ToCharArray()[0]);
		}

		public string[] NamedParameters
		{
			get
			{
				string[] retVal = new String[actualNamedParameters.Count];
				int i = 0;
				foreach (string parm in actualNamedParameters)
				{
					retVal[i++] = parm;
				}
				return retVal;
			}
		}

		public IQuery SetProperties(object bean)
		{
			System.Type clazz = bean.GetType();
			foreach (string namedParam in actualNamedParameters)
			{
				try
				{
					IGetter getter = ReflectHelper.GetGetter(clazz, namedParam, "property");
					SetParameter(namedParam, getter.Get(bean), GuessType(getter.ReturnType));
				}
				catch (Exception)
				{
				}
			}
			return this;
		}

		public virtual void SetLockMode(string alias, LockMode lockMode)
		{
			lockModes[alias] = lockMode;
		}

		public IDictionary LockModes
		{
			get { return lockModes; }
		}

		internal protected ISessionImplementor Session
		{
			get { return session; }
		}

		public T UniqueResult<T>()
		{
			object result = UniqueResult();
			if (result == null && typeof(T).IsValueType)
			{
				throw new InvalidCastException("UniqueResult<T>() cannot cast null result to value type. Call UniqueResult<T?>() instead");
			}
			else
			{
				return (T)UniqueResult();
			}
		}

		public object UniqueResult()
		{
			return UniqueElement(List());
		}

		internal static object UniqueElement(IList list)
		{
			int size = list.Count;
			if (size == 0)
			{
				return null;
			}
			object first = list[0];
			for (int i = 1; i < size; i++)
			{
				if (list[i] != first)
				{
					throw new NonUniqueResultException(size);
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
			return (IType[])types.ToArray(typeof(IType));
		}

		public virtual object[] ValueArray()
		{
			return (object[])values.ToArray(typeof(object));
		}

		public virtual QueryParameters GetQueryParameters()
		{
			return GetQueryParameters(NamedParams);
		}

		public virtual QueryParameters GetQueryParameters(IDictionary namedParams)
		{
			return new QueryParameters(
					TypeArray(),
					ValueArray(),
					namedParams,
					lockModes,
					selection,
					readOnly,
					cacheable,
					cacheRegion,
					string.Empty,
					collectionKey == null ? null : new object[] { collectionKey },
					optionalObject,
					optionalEntityName == null ? null : optionalEntityName.FullName,
					optionalId,
					resultTransformer);
		}

		public IQuery SetCacheable(bool cacheable)
		{
			this.cacheable = cacheable;
			return this;
		}

		public IQuery SetCacheRegion(string cacheRegion)
		{
			if (cacheRegion != null)
				this.cacheRegion = cacheRegion.Trim();
			return this;
		}

		public IQuery SetReadOnly(bool readOnly)
		{
			this.readOnly = readOnly;
			return this;
		}

		// Remaining methods of IQuery interface - left for children to implement
		public abstract IEnumerable Enumerable();
		public abstract IEnumerable<T> Enumerable<T>();
		public abstract IList List();
		public abstract void List(IList results);
		public abstract IList<T> List<T>();

		public void SetOptionalId(object optionalId)
		{
			this.optionalId = optionalId;
		}

		public void SetOptionalObject(object optionalObject)
		{
			this.optionalObject = optionalObject;
		}

		public void SetOptionalEntityName(System.Type optionalEntityName)
		{
			this.optionalEntityName = optionalEntityName;
		}

		public IQuery SetFlushMode(FlushMode flushMode)
		{
			this.flushMode = flushMode;
			return this;
		}

		protected void Before()
		{
			if (flushMode != FlushMode.Unspecified)
			{
				sessionFlushMode = Session.FlushMode;
				Session.FlushMode = flushMode;
			}
			if (cacheMode.HasValue)
			{
				sessionCacheMode = Session.CacheMode;
				Session.CacheMode = cacheMode.Value;
			}
		}

		protected void After()
		{
			if (sessionFlushMode != FlushMode.Unspecified)
			{
				Session.FlushMode = sessionFlushMode;
				sessionFlushMode = FlushMode.Unspecified;
			}
			if (sessionCacheMode.HasValue)
			{
				Session.CacheMode = sessionCacheMode.Value;
				sessionCacheMode = null;
			}
		}

		public IQuery SetCollectionKey(object collectionKey)
		{
			this.collectionKey = collectionKey;
			return this;
		}

		public IQuery SetResultTransformer(IResultTransformer transformer)
		{
			this.resultTransformer = transformer;
			return this;
		}

		/// <summary> Override the current session cache mode, just for this query.
		/// </summary>
		/// <param name="cacheMode">The cache mode to use. </param>
		/// <returns> this (for method chaining) </returns>
		public IQuery SetCacheMode(CacheMode cacheMode)
		{
			this.cacheMode = cacheMode;
			return this;

		}

		public IQuery SetIgnoreUknownNamedParameters(bool ignoredUnknownNamedParameters)
		{
			this.shouldIgnoredUnknownNamedParameters = ignoredUnknownNamedParameters;
			return this;
		}

		public override string ToString()
		{
			return queryString;
		}
	}
}

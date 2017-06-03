using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Hql;
using NHibernate.Properties;
using NHibernate.Proxy;
using NHibernate.Transform;
using NHibernate.Type;
using NHibernate.Util;
using System.Linq;
using System.Threading;

namespace NHibernate.Impl
{
	/// <summary>
	/// Abstract implementation of the IQuery interface.
	/// </summary>
	public abstract partial class AbstractQueryImpl : IQuery
	{
		private readonly string queryString;
		private readonly ISessionImplementor session;
		protected internal ParameterMetadata parameterMetadata;

		private readonly RowSelection selection;
		private readonly List<object> values = new List<object>(4);
		private readonly List<IType> types = new List<IType>(4);
		private readonly Dictionary<string, TypedValue> namedParameters = new Dictionary<string, TypedValue>(4);
		protected readonly Dictionary<string, TypedValue> namedParameterLists = new Dictionary<string, TypedValue>(4);
		private bool cacheable;
		private string cacheRegion;
		private bool? readOnly;
		private static readonly object UNSET_PARAMETER = new object();
		private static readonly IType UNSET_TYPE = null;
		private object optionalId;
		private object optionalObject;
		private string optionalEntityName;
		private FlushMode flushMode = FlushMode.Unspecified;
		private FlushMode sessionFlushMode = FlushMode.Unspecified;
		private object collectionKey;
		private IResultTransformer resultTransformer;
		private bool shouldIgnoredUnknownNamedParameters;
		private CacheMode? cacheMode;
		private CacheMode? sessionCacheMode;
		private string comment;

		protected AbstractQueryImpl(string queryString, FlushMode flushMode, ISessionImplementor session,
			ParameterMetadata parameterMetadata)
		{
			this.session = session;
			this.queryString = queryString;
			selection = new RowSelection();
			this.flushMode = flushMode;
			cacheMode = null;
			this.parameterMetadata = parameterMetadata;
		}

		public bool Cacheable
		{
			get { return cacheable; }
		}

		public string CacheRegion
		{
			get { return cacheRegion; }
		}

		public bool HasNamedParameters
		{
			get { return parameterMetadata.NamedParameterNames.Count > 0; }
		}

		protected internal virtual void VerifyParameters()
		{
			VerifyParameters(false);
		}

		/// <summary>
		/// Perform parameter validation.  Used prior to executing the encapsulated query.
		/// </summary>
		/// <param name="reserveFirstParameter">
		/// if true, the first ? will not be verified since
		/// its needed for e.g. callable statements returning a out parameter
		/// </param>
		protected internal virtual void VerifyParameters(bool reserveFirstParameter)
		{
			if (parameterMetadata.NamedParameterNames.Count != namedParameters.Count + namedParameterLists.Count)
			{
				var missingParams = new HashSet<string>(parameterMetadata.NamedParameterNames);
				missingParams.ExceptWith(namedParameterLists.Keys);
				missingParams.ExceptWith(namedParameters.Keys);
				throw new QueryException("Not all named parameters have been set: " + CollectionPrinter.ToString(missingParams), QueryString);
			}

			int positionalValueSpan = 0;
			for (int i = 0; i < values.Count; i++)
			{
				object obj = types[i];
				if (values[i] == UNSET_PARAMETER || obj == UNSET_TYPE)
				{
					if (reserveFirstParameter && i == 0)
					{
						continue;
					}
					else
					{
						throw new QueryException("Unset positional parameter at position: " + i, QueryString);
					}
				}
				positionalValueSpan++;
			}

			if (parameterMetadata.OrdinalParameterCount != positionalValueSpan)
			{
				if (reserveFirstParameter && parameterMetadata.OrdinalParameterCount - 1 != positionalValueSpan)
				{
					throw new QueryException(
						"Expected positional parameter count: " + (parameterMetadata.OrdinalParameterCount - 1) + ", actual parameters: "
						+ CollectionPrinter.ToString(values), QueryString);
				}
				else if (!reserveFirstParameter)
				{
					throw new QueryException(
						"Expected positional parameter count: " + parameterMetadata.OrdinalParameterCount + ", actual parameters: "
						+ CollectionPrinter.ToString(values), QueryString);
				}
			}
		}

		protected internal virtual IType DetermineType(int paramPosition, object paramValue, IType defaultType)
		{
			IType type = parameterMetadata.GetOrdinalParameterExpectedType(paramPosition + 1) ?? defaultType;
			return type;
		}

		protected internal virtual IType DetermineType(int paramPosition, object paramValue)
		{
			IType type = parameterMetadata.GetOrdinalParameterExpectedType(paramPosition + 1) ?? GuessType(paramValue);
			return type;
		}

		protected internal virtual IType DetermineType(string paramName, object paramValue, IType defaultType)
		{
			IType type = parameterMetadata.GetNamedParameterExpectedType(paramName) ?? defaultType;
			return type;
		}

		protected internal virtual IType DetermineType(string paramName, object paramValue)
		{
			IType type = parameterMetadata.GetNamedParameterExpectedType(paramName) ?? GuessType(paramValue);
			return type;
		}

		protected internal virtual IType DetermineType(string paramName, System.Type clazz)
		{
			IType type = parameterMetadata.GetNamedParameterExpectedType(paramName) ?? GuessType(clazz);
			return type;
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

			System.Type clazz = NHibernateProxyHelper.GetClassWithoutInitializingProxy(param);
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
					session.Factory.GetEntityPersister(clazz.FullName);
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

		/// <summary>
		/// Warning: adds new parameters to the argument by side-effect, as well as mutating the query string!
		/// </summary>
		protected internal virtual string ExpandParameterLists(IDictionary<string, TypedValue> namedParamsCopy)
		{
			string query = queryString;
			foreach (var me in namedParameterLists)
				query = ExpandParameterList(query, me.Key, me.Value, namedParamsCopy);

			return query;
		}

		/// <summary>
		/// Warning: adds new parameters to the argument by side-effect, as well as mutating the query string!
		/// </summary>
		private string ExpandParameterList(string query, string name, TypedValue typedList, IDictionary<string, TypedValue> namedParamsCopy)
		{
			var vals = (IEnumerable) typedList.Value;
			var type = typedList.Type;

			var typedValues = (from object value in vals
							   select new TypedValue(type, value))
				.ToList();

			if (typedValues.Count == 1)
			{
				namedParamsCopy[name] = typedValues[0];
				return query;
			}
			
			var isJpaPositionalParam = parameterMetadata.GetNamedParameterDescriptor(name).JpaStyle;
			var aliases = new string[typedValues.Count];
			for (var index = 0; index < typedValues.Count; index++)
			{
				var value = typedValues[index];
				var alias =  (isJpaPositionalParam ? 'x' + name : name + StringHelper.Underscore) + index + StringHelper.Underscore;
				namedParamsCopy[alias] = value;
				aliases[index] = ParserHelper.HqlVariablePrefix + alias;
			}

			var paramPrefix = isJpaPositionalParam ? StringHelper.SqlParameter : ParserHelper.HqlVariablePrefix;

			return StringHelper.Replace(query, paramPrefix + name, string.Join(StringHelper.CommaSpace, aliases), true);
		}

		#region Parameters

		public IQuery SetParameter(int position, object val, IType type)
		{
			CheckPositionalParameter(position);
			int size = values.Count;
			if (position < size)
			{
				values[position] = val;
				types[position] = type;
			}
			else
			{
				// prepend value and type list with null for any positions before the wanted position.
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
			if (!parameterMetadata.NamedParameterNames.Contains(name))
			{
				if (shouldIgnoredUnknownNamedParameters)//just ignore it
					return this;
				throw new ArgumentException("Parameter " + name + " does not exist as a named parameter in [" + QueryString + "]");
			}
			else
			{
				namedParameters[name] = new TypedValue(type, val);
				return this;
			}
		}

		public IQuery SetParameter<T>(int position, T val)
		{
			CheckPositionalParameter(position);

			return SetParameter(position, val, parameterMetadata.GetOrdinalParameterExpectedType(position + 1) ?? GuessType(typeof(T)));
		}

		private void CheckPositionalParameter(int position)
		{
			if (parameterMetadata.OrdinalParameterCount == 0)
			{
				throw new ArgumentException("No positional parameters in query: " + QueryString);
			}
			if (position < 0 || position > parameterMetadata.OrdinalParameterCount - 1)
			{
				throw new ArgumentException("Positional parameter does not exist: " + position + " in query: " + QueryString);
			}
		}

		public IQuery SetParameter<T>(string name, T val)
		{
			return SetParameter(name, val, parameterMetadata.GetNamedParameterExpectedType(name) ?? GuessType(typeof (T)));
		}

		public IQuery SetParameter(string name, object val)
		{
			if (!parameterMetadata.NamedParameterNames.Contains(name))
			{
				if (shouldIgnoredUnknownNamedParameters)//just ignore it
					return this;
			}

			if (val == null)
			{
				IType type = parameterMetadata.GetNamedParameterExpectedType(name);
				if (type == null)
				{
					throw new ArgumentNullException("val",
																					"A type specific Set(name, val) should be called because the Type can not be guessed from a null value.");
				}

				SetParameter(name, val, type);
			}
			else
			{
				SetParameter(name, val, DetermineType(name, val));
			}

			return this;
		}

		public IQuery SetParameter(int position, object val)
		{
			if (val == null)
			{
				throw new ArgumentNullException("val",
																				"A type specific Set(position, val) should be called because the Type can not be guessed from a null value.");
			}
			else
			{
				SetParameter(position, val, DetermineType(position, val));
			}
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

		public IQuery SetDateTimeOffset(string name, DateTimeOffset val)
		{
			SetParameter(name, val, NHibernateUtil.DateTimeOffset);
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

		public IQuery SetDateTime2(int position, DateTime val)
		{
			SetParameter(position, val, NHibernateUtil.DateTime2);
			return this;
		}

		public IQuery SetDateTime2(string name, DateTime val)
		{
			SetParameter(name, val, NHibernateUtil.DateTime2);
			return this;
		}

		public IQuery SetTimeSpan(int position, TimeSpan val)
		{
			SetParameter(position, val, NHibernateUtil.TimeSpan);
			return this;
		}

		public IQuery SetTimeSpan(string name, TimeSpan val)
		{
			SetParameter(name, val, NHibernateUtil.TimeSpan);
			return this;
		}

		public IQuery SetTimeAsTimeSpan(int position, TimeSpan val)
		{
			SetParameter(position, val, NHibernateUtil.TimeAsTimeSpan);
			return this;
		}

		public IQuery SetTimeAsTimeSpan(string name, TimeSpan val)
		{
			SetParameter(name, val, NHibernateUtil.TimeAsTimeSpan);
			return this;
		}

		public IQuery SetDateTimeOffset(int position, DateTimeOffset val)
		{
			SetParameter(position, val, NHibernateUtil.DateTimeOffset);
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

		public IQuery SetProperties(IDictionary map)
		{
			string[] @params = NamedParameters;
			for (int i = 0; i < @params.Length; i++)
			{
				var namedParam = @params[i];
				var obj = map[namedParam];
				if (obj == null)
				{
					continue;
				}
				if (obj is IEnumerable && !(obj is string))
				{
					SetParameterList(namedParam, (IEnumerable) obj);
				}
				else
				{
					SetParameter(namedParam, obj, DetermineType(namedParam, obj.GetType()));
				}
			}
			return this;
		}

		public IQuery SetProperties(object bean)
		{
			System.Type clazz = bean.GetType();
			string[] @params = NamedParameters;
			for (int i = 0; i < @params.Length; i++)
			{
				string namedParam = @params[i];
				try
				{
					var getter = ReflectHelper.GetGetter(clazz, namedParam, "property");
					var retType = getter.ReturnType;
					var obj = getter.Get(bean);
					if (typeof(IEnumerable).IsAssignableFrom(retType) && retType != typeof(string))
					{
						SetParameterList(namedParam, (IEnumerable) obj);
					}
					else
					{
						SetParameter(namedParam, obj, DetermineType(namedParam, retType));
					}
				}
				catch (PropertyNotFoundException)
				{
					// ignore
				}
			}
			return this;
		}

		public IQuery SetParameterList(string name, IEnumerable vals, IType type)
		{
			if (!parameterMetadata.NamedParameterNames.Contains(name))
			{
				if (shouldIgnoredUnknownNamedParameters)//just ignore it
					return this;

				throw new ArgumentException("Parameter " + name + " does not exist as a named parameter in [" + QueryString + "]");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type","Can't determine the type of parameter-list elements.");
			}
			if(!vals.Any())
			{
				throw new QueryException(string.Format("An empty parameter-list generates wrong SQL; parameter name '{0}'", name));
			}
			namedParameterLists[name] = new TypedValue(type, vals);
			return this;
		}

		public IQuery SetParameterList(string name, IEnumerable vals)
		{
			if (vals == null)
			{
				throw new ArgumentNullException("vals");
			}

			if (!parameterMetadata.NamedParameterNames.Contains(name))
			{
				if (shouldIgnoredUnknownNamedParameters)//just ignore it
					return this;
			}

			object firstValue = vals.FirstOrNull();
			SetParameterList(name, vals, firstValue == null ? GuessType(vals.GetCollectionElementType()) : DetermineType(name, firstValue));

			return this;
		}

		#endregion

		#region Query properties

		public string QueryString
		{
			get { return queryString; }
		}

		protected internal IDictionary<string, TypedValue> NamedParams
		{
			// NB The java one always returns a copy, so I'm going to reproduce that behaviour
			get { return new Dictionary<string, TypedValue>(namedParameters); }
		}

		protected IDictionary NamedParameterLists
		{
			get { return namedParameterLists; }
		}

		protected IList Values
		{
			get { return values; }
		}

		protected IList<IType> Types
		{
			get { return types; }
		}

		public virtual IType[] ReturnTypes
		{
			get { return session.Factory.GetReturnTypes(queryString); }
		}

		public virtual string[] ReturnAliases
		{
			get { return session.Factory.GetReturnAliases(queryString); }
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

		public string[] NamedParameters
		{
			get
			{
				return parameterMetadata.NamedParameterNames.ToArray();
			}
		}

		public abstract IQuery SetLockMode(string alias, LockMode lockMode);

		public IQuery SetComment(string comment)
		{
			this.comment = comment;
			return this;
		}

		internal protected ISessionImplementor Session
		{
			get { return session; }
		}

		protected RowSelection RowSelection
		{
			get { return selection; }
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

		/// <inheritdoc />
		public bool IsReadOnly
		{
			get { return readOnly == null ? Session.PersistenceContext.DefaultReadOnly : readOnly.Value; }
		}

		/// <inheritdoc />
		public IQuery SetReadOnly(bool readOnly)
		{
			this.readOnly = readOnly;
			return this;
		}

		public void SetOptionalId(object optionalId)
		{
			this.optionalId = optionalId;
		}

		public void SetOptionalObject(object optionalObject)
		{
			this.optionalObject = optionalObject;
		}

		public void SetOptionalEntityName(string optionalEntityName)
		{
			this.optionalEntityName = optionalEntityName;
		}

		public IQuery SetFlushMode(FlushMode flushMode)
		{
			this.flushMode = flushMode;
			return this;
		}

		public IQuery SetCollectionKey(object collectionKey)
		{
			this.collectionKey = collectionKey;
			return this;
		}

		public IQuery SetResultTransformer(IResultTransformer transformer)
		{
			resultTransformer = transformer;
			return this;
		}

		public IEnumerable<T> Future<T>()
		{
			if (!session.Factory.ConnectionProvider.Driver.SupportsMultipleQueries)
			{
				return List<T>();
			}

			session.FutureQueryBatch.Add<T>(this);
			return session.FutureQueryBatch.GetEnumerator<T>();
		}

		public IFutureValue<T> FutureValue<T>()
		{
			if (!session.Factory.ConnectionProvider.Driver.SupportsMultipleQueries)
			{
				return new FutureValue<T>(List<T>);
			}
			
			session.FutureQueryBatch.Add<T>(this);
			return session.FutureQueryBatch.GetFutureValue<T>();
		}

		public IAsyncEnumerable<T> FutureAsync<T>(CancellationToken cancellationToken)
		{
			if (!session.Factory.ConnectionProvider.Driver.SupportsMultipleQueries)
			{
				return new DelayedAsyncEnumerator<T>(async () => await ListAsync<T>(cancellationToken).ConfigureAwait(false));
			}

			session.FutureQueryBatch.Add<T>(this);
			return session.FutureQueryBatch.GetAsyncEnumerator<T>(cancellationToken);
		}

		public IFutureValueAsync<T> FutureValueAsync<T>(CancellationToken cancellationToken)
		{
			if (!session.Factory.ConnectionProvider.Driver.SupportsMultipleQueries)
			{
				return new FutureValueAsync<T>(async () => await ListAsync<T>(cancellationToken).ConfigureAwait(false));
			}

			session.FutureQueryBatch.Add<T>(this);
			return session.FutureQueryBatch.GetFutureValueAsync<T>(cancellationToken);
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
			shouldIgnoredUnknownNamedParameters = ignoredUnknownNamedParameters;
			return this;
		}

		protected internal abstract IDictionary<string, LockMode> LockModes { get;}

		#endregion

		#region Execution methods

		public abstract int ExecuteUpdate();
		public abstract IEnumerable Enumerable();
		public abstract IEnumerable<T> Enumerable<T>();
		public abstract IList List();
		public abstract void List(IList results);
		public abstract IList<T> List<T>();
		public T UniqueResult<T>()
		{
			object result = UniqueResult();
			if (result == null && typeof(T).IsValueType)
			{
				return default(T);
			}
			else
			{
				return (T)result;
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

		public virtual IType[] TypeArray()
		{
			return types.ToArray();
		}

		public virtual object[] ValueArray()
		{
			return values.ToArray();
		}

		public virtual QueryParameters GetQueryParameters()
		{
			return GetQueryParameters(NamedParams);
		}

		public virtual QueryParameters GetQueryParameters(IDictionary<string, TypedValue> namedParams)
		{
			return new QueryParameters(
					TypeArray(),
					ValueArray(),
					namedParams,
					LockModes,
					Selection,
					true,
					IsReadOnly,
					cacheable,
					cacheRegion,
					comment,
					collectionKey == null ? null : new[] { collectionKey },
					optionalObject,
					optionalEntityName,
					optionalId,
					resultTransformer);
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

		#endregion

		public override string ToString()
		{
			return queryString;
		}

		protected internal abstract IEnumerable<ITranslator> GetTranslators(ISessionImplementor sessionImplementor, QueryParameters queryParameters);
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Proxy;
using NHibernate.Transform;
using NHibernate.Type;

namespace NHibernate.Impl
{
	/// <summary>
	/// Base class to create queries in "detached mode" where the NHibernate session is not available.
	/// </summary>
	/// <seealso cref="IDetachedQuery"/>
	/// <seealso cref="NHibernate.Impl.AbstractQueryImpl"/>
	/// <remarks>
	/// The behaviour of each method is basically the same of <see cref="NHibernate.Impl.AbstractQueryImpl"/> methods.
	/// The main difference is on <see cref="SetProperties(object)"/>:
	/// If you mix <see cref="object"/> with named parameters setter, if same param name are found,
	/// the value of the parameter setter override the value read from the POCO.
	/// </remarks>
	[Serializable]
	public abstract class AbstractDetachedQuery : IDetachedQuery, IDetachedQueryImplementor
	{
		// Fields are protected for test scope
		// All information are hold local to have more probability to make AbstractDetachedQuery serializable without
		// touch any other NH class.
		// Another issue, to hold params locally, is that NH need session to discover some untyped param.
		// Parameters rules are delegated to IQuery implementation.
		// Untyped Parameters
		protected readonly Dictionary<int, object> posUntypeParams = new Dictionary<int, object>(4);
		protected readonly Dictionary<string, object> namedUntypeParams = new Dictionary<string, object>();
		protected readonly Dictionary<string, IEnumerable> namedUntypeListParams = new Dictionary<string, IEnumerable>(2);

		// Optional parameters are used for parameters values from bean.
		// The IQuery implementation use the actualNamedParameters to know which property it need.
		protected readonly IList optionalUntypeParams = new List<object>(2);

		// Typed Parameters
		protected readonly Dictionary<int, TypedValue> posParams = new Dictionary<int, TypedValue>(4);
		protected readonly Dictionary<string, TypedValue> namedParams = new Dictionary<string, TypedValue>();
		protected readonly Dictionary<string, TypedValue> namedListParams = new Dictionary<string, TypedValue>(2);

		// other query info
		protected readonly Dictionary<string, LockMode> lockModes = new Dictionary<string, LockMode>(2);
		protected readonly RowSelection selection = new RowSelection();
		protected bool cacheable;
		protected string cacheRegion;
		protected bool readOnly;
		protected FlushMode flushMode = FlushMode.Unspecified;
		protected IResultTransformer resultTransformer;
		protected bool shouldIgnoredUnknownNamedParameters;
		protected CacheMode? cacheMode;
		protected string comment;

		#region IDetachedQuery Members

		public abstract IQuery GetExecutableQuery(ISession session);

		public IDetachedQuery SetMaxResults(int maxResults)
		{
			selection.MaxRows = maxResults;
			return this;
		}

		public IDetachedQuery SetFirstResult(int firstResult)
		{
			selection.FirstRow = firstResult;
			return this;
		}

		public virtual IDetachedQuery SetComment(string comment)
		{
			this.comment = comment;
			return this;
		}

		public virtual IDetachedQuery SetCacheable(bool cacheable)
		{
			this.cacheable = cacheable;
			return this;
		}

		public virtual IDetachedQuery SetCacheRegion(string cacheRegion)
		{
			this.cacheRegion = cacheRegion;
			return this;
		}

		public virtual IDetachedQuery SetReadOnly(bool readOnly)
		{
			this.readOnly = readOnly;
			return this;
		}

		public virtual IDetachedQuery SetTimeout(int timeout)
		{
			selection.Timeout = timeout;
			return this;
		}

		public virtual IDetachedQuery SetFetchSize(int fetchSize)
		{
			selection.FetchSize = fetchSize;
			return this;
		}

		public void SetLockMode(string alias, LockMode lockMode)
		{
			if (string.IsNullOrEmpty(alias))
				throw new ArgumentNullException("alias", "Is null or empty.");

			lockModes[alias] = lockMode;
		}

		public IDetachedQuery SetParameter(int position, object val, IType type)
		{
			posParams[position] = new TypedValue(type, val, EntityMode.Poco);
			return this;
		}

		public IDetachedQuery SetParameter(string name, object val, IType type)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name", "Is null or empty.");
			namedParams[name] = new TypedValue(type, val, EntityMode.Poco);
			return this;
		}

		public IDetachedQuery SetParameter(int position, object val)
		{
			posUntypeParams[position] = val;
			return this;
		}

		public IDetachedQuery SetParameter(string name, object val)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name", "Is null or empty.");
			namedUntypeParams[name] = val;
			return this;
		}

		public IDetachedQuery SetParameterList(string name, IEnumerable vals, IType type)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name", "Is null or empty.");
			namedListParams[name] = new TypedValue(type, vals, EntityMode.Poco);
			return this;
		}

		public IDetachedQuery SetParameterList(string name, IEnumerable vals)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name", "Is null or empty.");
			namedUntypeListParams[name] = vals;
			return this;
		}

		public IDetachedQuery SetProperties(object obj)
		{
			if (obj == null)
				throw new ArgumentNullException("obj");
			optionalUntypeParams.Add(obj);
			return this;
		}

		public IDetachedQuery SetAnsiString(int position, string val)
		{
			SetParameter(position, val, NHibernateUtil.AnsiString);
			return this;
		}

		public IDetachedQuery SetAnsiString(string name, string val)
		{
			SetParameter(name, val, NHibernateUtil.AnsiString);
			return this;
		}

		public IDetachedQuery SetBinary(int position, byte[] val)
		{
			SetParameter(position, val, NHibernateUtil.Binary);
			return this;
		}

		public IDetachedQuery SetBinary(string name, byte[] val)
		{
			SetParameter(name, val, NHibernateUtil.Binary);
			return this;
		}

		public IDetachedQuery SetBoolean(int position, bool val)
		{
			SetParameter(position, val, NHibernateUtil.Boolean);
			return this;
		}

		public IDetachedQuery SetBoolean(string name, bool val)
		{
			SetParameter(name, val, NHibernateUtil.Boolean);
			return this;
		}

		public IDetachedQuery SetByte(int position, byte val)
		{
			SetParameter(position, val, NHibernateUtil.Byte);
			return this;
		}

		public IDetachedQuery SetByte(string name, byte val)
		{
			SetParameter(name, val, NHibernateUtil.Byte);
			return this;
		}

		public IDetachedQuery SetCharacter(int position, char val)
		{
			SetParameter(position, val, NHibernateUtil.Character);
			return this;
		}

		public IDetachedQuery SetCharacter(string name, char val)
		{
			SetParameter(name, val, NHibernateUtil.Character);
			return this;
		}

		public IDetachedQuery SetDateTime(int position, DateTime val)
		{
			SetParameter(position, val, NHibernateUtil.DateTime);
			return this;
		}

		public IDetachedQuery SetDateTime(string name, DateTime val)
		{
			SetParameter(name, val, NHibernateUtil.DateTime);
			return this;
		}

		public IDetachedQuery SetDecimal(int position, decimal val)
		{
			SetParameter(position, val, NHibernateUtil.Decimal);
			return this;
		}

		public IDetachedQuery SetDecimal(string name, decimal val)
		{
			SetParameter(name, val, NHibernateUtil.Decimal);
			return this;
		}

		public IDetachedQuery SetDouble(int position, double val)
		{
			SetParameter(position, val, NHibernateUtil.Double);
			return this;
		}

		public IDetachedQuery SetDouble(string name, double val)
		{
			SetParameter(name, val, NHibernateUtil.Double);
			return this;
		}

		public IDetachedQuery SetEntity(int position, object val)
		{
			SetParameter(position, val, NHibernateUtil.Entity(NHibernateProxyHelper.GuessClass(val)));
			return this;
		}

		public IDetachedQuery SetEntity(string name, object val)
		{
			SetParameter(name, val, NHibernateUtil.Entity(NHibernateProxyHelper.GuessClass(val)));
			return this;
		}

		public IDetachedQuery SetEnum(int position, Enum val)
		{
			SetParameter(position, val, NHibernateUtil.Enum(val.GetType()));
			return this;
		}

		public IDetachedQuery SetEnum(string name, Enum val)
		{
			SetParameter(name, val, NHibernateUtil.Enum(val.GetType()));
			return this;
		}

		public IDetachedQuery SetInt16(int position, short val)
		{
			SetParameter(position, val, NHibernateUtil.Int16);
			return this;
		}

		public IDetachedQuery SetInt16(string name, short val)
		{
			SetParameter(name, val, NHibernateUtil.Int16);
			return this;
		}

		public IDetachedQuery SetInt32(int position, int val)
		{
			SetParameter(position, val, NHibernateUtil.Int32);
			return this;
		}

		public IDetachedQuery SetInt32(string name, int val)
		{
			SetParameter(name, val, NHibernateUtil.Int32);
			return this;
		}

		public IDetachedQuery SetInt64(int position, long val)
		{
			SetParameter(position, val, NHibernateUtil.Int64);
			return this;
		}

		public IDetachedQuery SetInt64(string name, long val)
		{
			SetParameter(name, val, NHibernateUtil.Int64);
			return this;
		}

		public IDetachedQuery SetSingle(int position, float val)
		{
			SetParameter(position, val, NHibernateUtil.Single);
			return this;
		}

		public IDetachedQuery SetSingle(string name, float val)
		{
			SetParameter(name, val, NHibernateUtil.Single);
			return this;
		}

		public IDetachedQuery SetString(int position, string val)
		{
			SetParameter(position, val, NHibernateUtil.String);
			return this;
		}

		public IDetachedQuery SetString(string name, string val)
		{
			SetParameter(name, val, NHibernateUtil.String);
			return this;
		}

		public IDetachedQuery SetTime(int position, DateTime val)
		{
			SetParameter(position, val, NHibernateUtil.Time);
			return this;
		}

		public IDetachedQuery SetTime(string name, DateTime val)
		{
			SetParameter(name, val, NHibernateUtil.Time);
			return this;
		}

		public IDetachedQuery SetTimestamp(int position, DateTime val)
		{
			SetParameter(position, val, NHibernateUtil.Timestamp);
			return this;
		}

		public IDetachedQuery SetTimestamp(string name, DateTime val)
		{
			SetParameter(name, val, NHibernateUtil.Timestamp);
			return this;
		}

		public IDetachedQuery SetGuid(int position, Guid val)
		{
			SetParameter(position, val, NHibernateUtil.Guid);
			return this;
		}

		public IDetachedQuery SetGuid(string name, Guid val)
		{
			SetParameter(name, val, NHibernateUtil.Guid);
			return this;
		}

		public virtual IDetachedQuery SetFlushMode(FlushMode flushMode)
		{
			this.flushMode = flushMode;
			return this;
		}

		public IDetachedQuery SetResultTransformer(IResultTransformer resultTransformer)
		{
			this.resultTransformer = resultTransformer;
			return this;
		}

		public IDetachedQuery SetIgnoreUknownNamedParameters(bool ignoredUnknownNamedParameters)
		{
			shouldIgnoredUnknownNamedParameters = ignoredUnknownNamedParameters;
			return this;
		}

		/// <summary> Override the current session cache mode, just for this query. </summary>
		/// <param name="cacheMode">The cache mode to use. </param>
		/// <returns> this (for method chaining) </returns>
		public virtual IDetachedQuery SetCacheMode(CacheMode cacheMode)
		{
			this.cacheMode = cacheMode;
			return this;
		}

		#endregion

		/// <summary>
		/// Fill all <see cref="IQuery"/> properties.
		/// </summary>
		/// <param name="q">The <see cref="IQuery"/>.</param>
		/// <remarks>
		/// Query properties are overriden/merged.
		/// </remarks>
		protected void SetQueryProperties(IQuery q)
		{
			q.SetMaxResults(selection.MaxRows)
				.SetFirstResult(selection.FirstRow)
				.SetCacheable(cacheable)
				.SetReadOnly(readOnly)
				.SetTimeout(selection.Timeout)
				.SetFlushMode(flushMode)
				.SetFetchSize(selection.FetchSize);
			if (!string.IsNullOrEmpty(comment))
				q.SetComment(comment);
			if (!string.IsNullOrEmpty(cacheRegion))
				q.SetCacheRegion(cacheRegion);
			if (resultTransformer != null)
				q.SetResultTransformer(resultTransformer);
			if (cacheMode.HasValue)
				q.SetCacheMode(cacheMode.Value);
			foreach (KeyValuePair<string, LockMode> mode in lockModes)
				q.SetLockMode(mode.Key, mode.Value);

			// Set AbstractQueryImpl property before set parameters
			var aqi = q as AbstractQueryImpl;
			if (aqi != null)
				aqi.SetIgnoreUknownNamedParameters(shouldIgnoredUnknownNamedParameters);

			// Even if the probably that somebody use a mixed technique to set parameters 
			// (from POCO using SetProperties and using named parameter setters) here is a possible
			// difference between IQuery and DetachedQuery behaviour.
			// In IQuery we don't know who override a param value; in DetachedQuery the direct use of 
			// a named parameter setter override the param value set by SetProperties(POCO)
			foreach (var obj in optionalUntypeParams)
				q.SetProperties(obj);

			// Set untyped positional parameters
			foreach (var pup in posUntypeParams)
				q.SetParameter(pup.Key, pup.Value);

			// Set untyped named parameters
			foreach (var nup in namedUntypeParams)
				q.SetParameter(nup.Key, nup.Value);

			// Set untyped named parameters list
			foreach (var nulp in namedUntypeListParams)
				q.SetParameterList(nulp.Key, nulp.Value);

			// Set typed positional parameters
			foreach (var pp in posParams)
				q.SetParameter(pp.Key, pp.Value.Value, pp.Value.Type);

			// Set typed named parameters
			foreach (var np in namedParams)
				q.SetParameter(np.Key, np.Value.Value, np.Value.Type);

			// Set typed named parameters List
			foreach (var nlp in namedListParams)
				q.SetParameterList(nlp.Key, (IEnumerable) nlp.Value.Value, nlp.Value.Type);
		}

		private void Reset()
		{
			ClearParameters();
			lockModes.Clear();
			selection.FirstRow = RowSelection.NoValue;
			selection.MaxRows = RowSelection.NoValue;
			selection.Timeout = RowSelection.NoValue;
			selection.FetchSize = RowSelection.NoValue;
			cacheable = false;
			cacheRegion = null;
			cacheMode = null;
			readOnly = false;
			flushMode = FlushMode.Unspecified;
			resultTransformer = null;
			shouldIgnoredUnknownNamedParameters = false;
			comment = null;
		}

		private void ClearParameters()
		{
			posUntypeParams.Clear();
			namedUntypeParams.Clear();
			namedUntypeListParams.Clear();
			optionalUntypeParams.Clear();
			posParams.Clear();
			namedParams.Clear();
			namedListParams.Clear();
		}

		#region IDetachedQueryImplementor Members

		/// <summary>
		/// Copy all properties to a given <see cref="IDetachedQuery"/>.
		/// </summary>
		/// <param name="destination">The given <see cref="IDetachedQuery"/>.</param>
		/// <remarks>
		/// The method use <see cref="IDetachedQuery"/> to set properties of <paramref name="destination"/>.
		/// </remarks>
		public void CopyTo(IDetachedQuery destination)
		{
			destination.SetMaxResults(selection.MaxRows)
				.SetFirstResult(selection.FirstRow)
				.SetCacheable(cacheable)
				.SetReadOnly(readOnly)
				.SetTimeout(selection.Timeout)
				.SetFlushMode(flushMode)
				.SetFetchSize(selection.FetchSize);
			if (!string.IsNullOrEmpty(comment))
				destination.SetComment(comment);
			if (!string.IsNullOrEmpty(cacheRegion))
				destination.SetCacheRegion(cacheRegion);
			if (cacheMode.HasValue)
				destination.SetCacheMode(cacheMode.Value);
			if (resultTransformer != null)
				destination.SetResultTransformer(resultTransformer);
			foreach (KeyValuePair<string, LockMode> mode in lockModes)
				destination.SetLockMode(mode.Key, mode.Value);

			SetParametersTo(destination);
		}

		/// <summary>
		/// Set only parameters to a given <see cref="IDetachedQuery"/>.
		/// </summary>
		/// <param name="destination">The given <see cref="IDetachedQuery"/>.</param>
		/// <remarks>
		/// The method use <see cref="IDetachedQuery"/> to set properties of <paramref name="destination"/>.
		/// Existing parameters in <paramref name="destination"/> are merged/overriden.
		/// </remarks>
		public void SetParametersTo(IDetachedQuery destination)
		{
			foreach (var obj in optionalUntypeParams)
				destination.SetProperties(obj);

			// Set untyped positional parameters
			foreach (var pup in posUntypeParams)
				destination.SetParameter(pup.Key, pup.Value);

			// Set untyped named parameters
			foreach (var nup in namedUntypeParams)
				destination.SetParameter(nup.Key, nup.Value);

			// Set untyped named parameters list
			foreach (var nulp in namedUntypeListParams)
				destination.SetParameterList(nulp.Key, nulp.Value);

			// Set typed positional parameters
			foreach (var pp in posParams)
				destination.SetParameter(pp.Key, pp.Value.Value, pp.Value.Type);

			// Set typed named parameters
			foreach (var np in namedParams)
				destination.SetParameter(np.Key, np.Value.Value, np.Value.Type);

			// Set typed named parameters List
			foreach (var nlp in namedListParams)
				destination.SetParameterList(nlp.Key, (IEnumerable) nlp.Value.Value, nlp.Value.Type);
		}

		void IDetachedQueryImplementor.OverrideInfoFrom(IDetachedQueryImplementor origin)
		{
			Reset();
			origin.CopyTo(this);
		}

		void IDetachedQueryImplementor.OverrideParametersFrom(IDetachedQueryImplementor origin)
		{
			ClearParameters();
			origin.SetParametersTo(this);
		}

		#endregion

		/// <summary>
		/// Clear all existing parameters and copy new parameters from a given origin.
		/// </summary>
		/// <param name="origin">The origin of parameters.</param>
		/// <returns>The current instance</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="origin"/> is null.</exception>
		public IDetachedQuery CopyParametersFrom(IDetachedQueryImplementor origin)
		{
			if (origin == null)
				throw new ArgumentNullException("origin");
			(this as IDetachedQueryImplementor).OverrideParametersFrom(origin);
			return this;
		}

	}
}

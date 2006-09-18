using System;
using System.Collections;

namespace NHibernate.Engine
{
	[Serializable]
	public class NamedQueryDefinition
	{
		private readonly string query;
		private readonly bool cacheable;
		private readonly string cacheRegion;
		private readonly int timeout = -1;
		private readonly int fetchSize = -1;
		private readonly FlushMode flushMode = FlushMode.Unspecified;
		private readonly IDictionary parameterTypes;

		//private CacheMode cacheMode;
		private bool readOnly;
		private string comment;

		public NamedQueryDefinition(
				string query,
				bool cacheable,
				string cacheRegion,
				int timeout,
				int fetchSize,
				FlushMode flushMode,
				//CacheMode cacheMode,
				bool readOnly,
				string comment,
				IDictionary parameterTypes
		)
		{
			this.query = query;
			this.cacheable = cacheable;
			this.cacheRegion = cacheRegion;
			this.timeout = timeout;
			this.fetchSize = fetchSize;
			this.flushMode = flushMode;
			this.parameterTypes = parameterTypes;
			//this.cacheMode = cacheMode;
			this.readOnly = readOnly;
			this.comment = comment;
		}

		public string QueryString
		{
			get { return query; }
		}

		public bool IsCacheable
		{
			get { return cacheable; }
		}

		public string CacheRegion
		{
			get { return cacheRegion; }
		}

		public int FetchSize
		{
			get { return fetchSize; }
		}

		public int Timeout
		{
			get { return timeout; }
		}

		public FlushMode FlushMode
		{
		    get { return flushMode; }
		}

		public override string ToString()
		{
			return GetType().FullName + '(' + query + ')';
		}

		public IDictionary ParameterTypes
		{
			get { return parameterTypes; }
		}

		public string Query
		{
			get { return query; }
		}

		//public CacheMode CacheMode
		//{
		//    get { return cacheMode; }
		//}

		public bool IsReadOnly
		{
			get { return readOnly; }
		}

		public string Comment
		{
			get { return comment; }
		}
	}
}

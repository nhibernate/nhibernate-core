using System;
using System.Collections;
using NHibernate.Loader.Custom;

namespace NHibernate.Engine
{
	[Serializable]
	public class NamedSQLQueryDefinition : NamedQueryDefinition
	{
		private ISQLQueryReturn[] queryReturns;
		private readonly IList querySpaces;
		private readonly bool callable;
		private string resultSetRef;

		public NamedSQLQueryDefinition(
			string query,
			ISQLQueryReturn[] queryReturns,
			IList querySpaces,
			bool cacheable,
			string cacheRegion,
			int timeout,
			int fetchSize,
			FlushMode flushMode,
			bool readOnly,
			string comment,
			IDictionary parameterTypes,
			bool callable)
			: base(
				query.Trim(), /* trim done to workaround stupid oracle bug that cant handle whitespaces before a { in a sp */
				cacheable,
				cacheRegion,
				timeout,
				fetchSize,
				flushMode,
				//cacheMode,
				readOnly,
				comment,
				parameterTypes
				)
		{
			this.queryReturns = queryReturns;
			this.querySpaces = querySpaces;
			this.callable = callable;
		}

		public NamedSQLQueryDefinition(
			string query,
			string resultSetRef,
			IList querySpaces,
			bool cacheable,
			string cacheRegion,
			int timeout,
			int fetchSize,
			FlushMode flushMode,
			//CacheMode cacheMode,
			bool readOnly,
			string comment,
			IDictionary parameterTypes,
			bool callable)
			: base(
				query.Trim(), /* trim done to workaround stupid oracle bug that cant handle whitespaces before a { in a sp */
				cacheable,
				cacheRegion,
				timeout,
				fetchSize,
				flushMode,
				//cacheMode,
				readOnly,
				comment,
				parameterTypes
				)
		{
			this.resultSetRef = resultSetRef;
			this.querySpaces = querySpaces;
			this.callable = callable;
		}

		public ISQLQueryReturn[] QueryReturns
		{
			get { return queryReturns; }
		}

		public IList QuerySpaces
		{
			get { return querySpaces; }
		}

		public bool IsCallable
		{
			get { return callable; }
		}

		public string ResultSetRef
		{
			get { return resultSetRef; }
		}
	}
}
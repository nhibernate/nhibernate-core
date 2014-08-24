using System;
using System.Collections;
using NHibernate.Engine.Query.Sql;
using System.Collections.Generic;

namespace NHibernate.Engine
{
	[Serializable]
	public class NamedSQLQueryDefinition : NamedQueryDefinition
	{
		private readonly INativeSQLQueryReturn[] queryReturns;
		private readonly IList<string> querySpaces;
		private readonly bool callable;
		private readonly string resultSetRef;

		public NamedSQLQueryDefinition(
			string query,
			INativeSQLQueryReturn[] queryReturns,
			IList<string> querySpaces,
			bool cacheable,
			string cacheRegion,
			int timeout,
			int fetchSize,
			FlushMode flushMode,
			CacheMode? cacheMode,
			bool readOnly,
			string comment,
			IDictionary<string, string> parameterTypes,
			bool callable)
			: base(
				query.Trim(), /* trim done to workaround stupid oracle bug that cant handle whitespaces before a { in a sp */
				cacheable,
				cacheRegion,
				timeout,
				fetchSize,
				flushMode,
				cacheMode,
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
			IList<string> querySpaces,
			bool cacheable,
			string cacheRegion,
			int timeout,
			int fetchSize,
			FlushMode flushMode,
			CacheMode? cacheMode,
			bool readOnly,
			string comment,
			IDictionary<string, string> parameterTypes,
			bool callable)
			: base(
				query.Trim(), /* trim done to workaround stupid oracle bug that cant handle whitespaces before a { in a sp */
				cacheable,
				cacheRegion,
				timeout,
				fetchSize,
				flushMode,
				cacheMode,
				readOnly,
				comment,
				parameterTypes
				)
		{
			this.resultSetRef = resultSetRef;
			this.querySpaces = querySpaces;
			this.callable = callable;
		}

		public INativeSQLQueryReturn[] QueryReturns
		{
			get { return queryReturns; }
		}

		public IList<string> QuerySpaces
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
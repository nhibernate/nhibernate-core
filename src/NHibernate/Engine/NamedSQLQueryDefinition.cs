using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using NHibernate.Engine.Query.Sql;
using NHibernate.Util;


namespace NHibernate.Engine
{
	[Serializable]
	public class NamedSQLQueryDefinition : NamedQueryDefinition
	{
		private readonly IReadOnlyList<INativeSQLQueryReturn> queryReturns;
		private readonly IReadOnlyList<string> querySpaces;
		private readonly bool callable;
		private readonly string resultSetRef;

		public NamedSQLQueryDefinition(
			string query,
			IList<INativeSQLQueryReturn> queryReturns,
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
			this.queryReturns = new ReadOnlyCollection<INativeSQLQueryReturn>(queryReturns);
			this.querySpaces = new ReadOnlyCollection<string>(querySpaces);
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
	    : base(query.Trim(),
	           /* trim done to workaround stupid oracle bug that cant handle whitespaces before a { in a sp */
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
	    this.queryReturns = ReadOnlyCollectionHelper.EmptyQueryReturns;
	    this.resultSetRef = resultSetRef;
	    this.querySpaces = new ReadOnlyCollection<string>(querySpaces);
	    this.callable = callable;
	  }

	  public IReadOnlyList<INativeSQLQueryReturn> QueryReturns
		{
			get { return queryReturns; }
		}

		public IReadOnlyList<string> QuerySpaces
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
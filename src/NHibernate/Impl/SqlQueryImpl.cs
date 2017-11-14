using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Engine.Query.Sql;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Impl
{
	/// <summary>
	/// Implements SQL query passthrough
	/// </summary>
	/// <example>
	/// An example mapping is:
	/// <code>
	/// &lt;sql-query-name name="mySqlQuery"&gt;
	/// &lt;return alias="person" class="eg.Person" /&gt;
	///		SELECT {person}.NAME AS {person.name}, {person}.AGE AS {person.age}, {person}.SEX AS {person.sex}
	///		FROM PERSON {person} WHERE {person}.NAME LIKE 'Hiber%'
	/// &lt;/sql-query-name&gt;
	/// </code>
	/// </example>
	public partial class SqlQueryImpl : AbstractQueryImpl, ISQLQuery
	{
		private readonly IList<INativeSQLQueryReturn> queryReturns;
		private readonly ICollection<string> querySpaces;
		private readonly bool callable;
		private bool autoDiscoverTypes;

		/// <summary> Constructs a SQLQueryImpl given a sql query defined in the mappings. </summary>
		/// <param name="queryDef">The representation of the defined sql-query. </param>
		/// <param name="session">The session to which this SQLQueryImpl belongs. </param>
		/// <param name="parameterMetadata">Metadata about parameters found in the query. </param>
		internal SqlQueryImpl(NamedSQLQueryDefinition queryDef, ISessionImplementor session, ParameterMetadata parameterMetadata)
			: base(queryDef.QueryString, queryDef.FlushMode, session, parameterMetadata)
		{
			if (!string.IsNullOrEmpty(queryDef.ResultSetRef))
			{
				ResultSetMappingDefinition definition = session.Factory.GetResultSetMapping(queryDef.ResultSetRef);
				if (definition == null)
				{
					throw new MappingException("Unable to find resultset-ref definition: " + queryDef.ResultSetRef);
				}
				queryReturns = new List<INativeSQLQueryReturn>(definition.GetQueryReturns());
			}
			else
			{
				queryReturns = new List<INativeSQLQueryReturn>(queryDef.QueryReturns);
			}

			querySpaces = queryDef.QuerySpaces;
			callable = queryDef.IsCallable;
		}

		internal SqlQueryImpl(string sql, IList<INativeSQLQueryReturn> queryReturns, ICollection<string> querySpaces, FlushMode flushMode, bool callable, ISessionImplementor session, ParameterMetadata parameterMetadata)
			: base(sql, flushMode, session, parameterMetadata)
		{
			this.queryReturns = queryReturns;
			this.querySpaces = querySpaces;
			this.callable = callable;
		}

		internal SqlQueryImpl(string sql, string[] returnAliases, System.Type[] returnClasses, LockMode[] lockModes, ISessionImplementor session, ICollection<string> querySpaces, FlushMode flushMode, ParameterMetadata parameterMetadata)
			: base(sql, flushMode, session, parameterMetadata)
		{
			queryReturns = new List<INativeSQLQueryReturn>(returnAliases.Length);
			for (int i = 0; i < returnAliases.Length; i++)
			{
				NativeSQLQueryRootReturn ret =
					new NativeSQLQueryRootReturn(returnAliases[i], returnClasses[i].FullName,
												 lockModes == null ? LockMode.None : lockModes[i]);
				queryReturns.Add(ret);
			}
			this.querySpaces = querySpaces;
			callable = false;
		}

		internal SqlQueryImpl(string sql, string[] returnAliases, System.Type[] returnClasses, ISessionImplementor session, ParameterMetadata parameterMetadata)
			: this(sql, returnAliases, returnClasses, null, session, null, FlushMode.Unspecified, parameterMetadata) { }

		internal SqlQueryImpl(string sql, ISessionImplementor session, ParameterMetadata parameterMetadata)
			: base(sql, FlushMode.Unspecified, session, parameterMetadata)
		{
			queryReturns = new List<INativeSQLQueryReturn>();
			querySpaces = null;
			callable = false;
		}

		protected internal override IDictionary<string, LockMode> LockModes
		{
			get
			{
				//we never need to apply locks to the SQL
				return new CollectionHelper.EmptyMapClass<string, LockMode>();
			}
		}

		private INativeSQLQueryReturn[] GetQueryReturns()
		{
			INativeSQLQueryReturn[] result = new INativeSQLQueryReturn[queryReturns.Count];
			queryReturns.CopyTo(result, 0);
			return result;
		}

		public override string[] ReturnAliases
		{
			get { throw new NotSupportedException("SQL queries do not currently support returning aliases"); }
		}

		public override IType[] ReturnTypes
		{
			get { throw new NotSupportedException("not yet implemented for SQL queries"); }
		}

		public override IList List()
		{
			VerifyParameters();
			IDictionary<string, TypedValue> namedParams = NamedParams;
			NativeSQLQuerySpecification spec = GenerateQuerySpecification(namedParams);
			QueryParameters qp = GetQueryParameters(namedParams);

			Before();
			try
			{
				return Session.List(spec, qp);
			}
			finally
			{
				After();
			}
		}

		public override void List(IList results)
		{
			VerifyParameters();
			IDictionary<string, TypedValue> namedParams = NamedParams;
			NativeSQLQuerySpecification spec = GenerateQuerySpecification(namedParams);
			QueryParameters qp = GetQueryParameters(namedParams);

			Before();
			try
			{
				Session.List(spec, qp, results);
			}
			finally
			{
				After();
			}
		}

		public override IList<T> List<T>()
		{
			VerifyParameters();
			IDictionary<string, TypedValue> namedParams = NamedParams;
			NativeSQLQuerySpecification spec = GenerateQuerySpecification(namedParams);
			QueryParameters qp = GetQueryParameters(namedParams);

			Before();
			try
			{
				return Session.List<T>(spec, qp);
			}
			finally
			{
				After();
			}
		}

		public NativeSQLQuerySpecification GenerateQuerySpecification(IDictionary<string, TypedValue> parameters)
		{
			return new NativeSQLQuerySpecification(
				ExpandParameterLists(parameters),
				GetQueryReturns(),
				querySpaces);
		}

		public override QueryParameters GetQueryParameters(IDictionary<string, TypedValue> namedParams)
		{
			QueryParameters qp = base.GetQueryParameters(namedParams);
			qp.Callable = callable;
			qp.HasAutoDiscoverScalarTypes = autoDiscoverTypes;
			return qp;
		}

		public override IEnumerable Enumerable()
		{
			throw new NotSupportedException("SQL queries do not currently support enumeration");
		}

		public override IEnumerable<T> Enumerable<T>()
		{
			throw new NotSupportedException("SQL queries do not currently support enumeration");
		}

		public ISQLQuery AddScalar(string columnAlias, IType type)
		{
			autoDiscoverTypes = true;
			queryReturns.Add(new NativeSQLQueryScalarReturn(columnAlias, type));
			return this;
		}

		public ISQLQuery AddJoin(string alias, string path)
		{
			return AddJoin(alias, path, LockMode.Read);
		}

		public ISQLQuery AddEntity(System.Type entityClass)
		{
			return AddEntity(entityClass.Name, entityClass);
		}

		public ISQLQuery AddEntity(string entityName)
		{
			return AddEntity(StringHelper.Unqualify(entityName), entityName);
		}

		public ISQLQuery AddEntity(string alias, string entityName)
		{
			return AddEntity(alias, entityName, LockMode.Read);
		}

		public ISQLQuery AddEntity(string alias, System.Type entityClass)
		{
			return AddEntity(alias, entityClass.FullName);
		}

		public ISQLQuery AddJoin(string alias, string path, LockMode lockMode)
		{
			int loc = path.IndexOf('.');
			if (loc < 0)
			{
				throw new QueryException("not a property path: " + path);
			}
			string ownerAlias = path.Substring(0, loc);
			string role = path.Substring(loc + 1);
			queryReturns.Add(
				new NativeSQLQueryJoinReturn(alias, ownerAlias, role, new CollectionHelper.EmptyMapClass<string, string[]>(), lockMode));
			return this;
		}

		public ISQLQuery AddEntity(string alias, string entityName, LockMode lockMode)
		{
			queryReturns.Add(new NativeSQLQueryRootReturn(alias, entityName, lockMode));
			return this;
		}

		public ISQLQuery AddEntity(string alias, System.Type entityClass, LockMode lockMode)
		{
			return AddEntity(alias, entityClass.FullName, lockMode);
		}

		public ISQLQuery SetResultSetMapping(string name)
		{
			ResultSetMappingDefinition mapping = Session.Factory.GetResultSetMapping(name);
			if (mapping == null)
			{
				throw new MappingException("Unknown SqlResultSetMapping [" + name + "]");
			}
			INativeSQLQueryReturn[] returns = mapping.GetQueryReturns();
			int length = returns.Length;
			for (int index = 0; index < length; index++)
			{
				queryReturns.Add(returns[index]);
			}
			return this;
		}

		protected internal override void VerifyParameters()
		{
			base.VerifyParameters();
			bool noReturns = queryReturns == null || queryReturns.Count == 0;
			if (noReturns)
			{
				autoDiscoverTypes = noReturns;
			}
			else
			{
				foreach (INativeSQLQueryReturn rtn in queryReturns)
				{
					if (rtn is NativeSQLQueryScalarReturn)
					{
						NativeSQLQueryScalarReturn scalar = (NativeSQLQueryScalarReturn) rtn;
						if (scalar.Type == null)
						{
							autoDiscoverTypes = true;
							break;
						}
					}
				}
			}

		}

		public override IQuery SetLockMode(string alias, LockMode lockMode)
		{
			throw new NotSupportedException("cannot set the lock mode for a native SQL query");
		}

		public override int ExecuteUpdate()
		{
			IDictionary<string,TypedValue> namedParams = NamedParams;
			Before();
			try
			{
				return Session.ExecuteNativeUpdate(GenerateQuerySpecification(namedParams), GetQueryParameters(namedParams));
			}
			finally
			{
				After();
			}
		}

		protected internal override IEnumerable<ITranslator> GetTranslators(ISessionImplementor sessionImplementor, QueryParameters queryParameters)
		{
			// NOTE: updates queryParameters.NamedParameters as (desired) side effect
			ExpandParameterLists(queryParameters.NamedParameters);

			var sqlQuery = this as ISQLQuery;
			yield return new SqlTranslator(sqlQuery, sessionImplementor.Factory);
		}
	}
}

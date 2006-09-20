using System;
using System.Collections;
#if NET_2_0
using System.Collections.Generic;
#endif

using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Loader.Custom;
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
	public class SqlQueryImpl : AbstractQueryImpl, ISQLQuery
	{
		private readonly IList queryReturns;
		private readonly ICollection querySpaces;
		private bool callable;

		private SqlQueryImpl(
			string sql,
			string[] returnAliases,
			System.Type[] returnClasses,
			LockMode[] lockModes,
			ISessionImplementor session,
			ICollection querySpaces,
			FlushMode flushMode)
			: base(sql, flushMode, session)
		{
			// TODO : this constructor form is *only* used from constructor directly below us; can it go away?
			queryReturns = new ArrayList(returnAliases.Length);
			for (int i = 0; i < returnAliases.Length; i++)
			{
				SQLQueryRootReturn ret = new SQLQueryRootReturn(
					returnAliases[i],
					returnClasses[i].AssemblyQualifiedName,
					lockModes == null ? LockMode.None : lockModes[i]);
				queryReturns.Add(ret);
			}

			this.querySpaces = querySpaces;
			this.callable = false;
		}

		public SqlQueryImpl(string sql, string[] returnAliases, System.Type[] returnClasses, ISessionImplementor session, ICollection querySpaces)
			: this(sql, returnAliases, returnClasses, null, session, querySpaces, FlushMode.Unspecified)
		{
		}

		public SqlQueryImpl(NamedSQLQueryDefinition queryDef, ISessionImplementor session)
			: base(queryDef.QueryString, queryDef.FlushMode, session)
		{
			if (queryDef.ResultSetRef != null)
			{
				ResultSetMappingDefinition definition = session.Factory
						.GetResultSetMapping(queryDef.ResultSetRef);
				if (definition == null)
				{
					throw new MappingException(
							"Unable to find resultset-ref definition: " +
							queryDef.ResultSetRef
						);
				}
				this.queryReturns = definition.GetQueryReturns();
			}
			else
			{
				this.queryReturns = queryDef.QueryReturns;
			}

			this.querySpaces = queryDef.QuerySpaces;
			this.callable = queryDef.IsCallable;
		}

		public SqlQueryImpl(string sql, ISessionImplementor session)
			: base(sql, FlushMode.Unspecified, session)
		{
			queryReturns = new ArrayList();
			querySpaces = null;
			callable = false;
		}

		private ISQLQueryReturn[] GetQueryReturns()
		{
			ISQLQueryReturn[] result = new ISQLQueryReturn[queryReturns.Count];
			queryReturns.CopyTo(result, 0);
			return result;
		}

		//public string[] ReturnAliases
		//{
		//    get { throw new NotSupportedException( "SQL queries do not currently support returning aliases" ); }
		//}

		//public override IType[] ReturnTypes
		//{
		//    get { throw new NotSupportedException( "not yet implemented for SQL queries" ); }
		//}

		public override IList List()
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
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
			IDictionary namedParams = NamedParams;
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

#if NET_2_0
		public override IList<T> List<T>()
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
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
#endif

		public NativeSQLQuerySpecification GenerateQuerySpecification(IDictionary parameters)
		{
			return new NativeSQLQuerySpecification(
				BindParameterLists(NamedParams),
				GetQueryReturns(),
				querySpaces);
		}

		public override QueryParameters GetQueryParameters(IDictionary namedParams)
		{
			QueryParameters qp = base.GetQueryParameters(namedParams);
			qp.Callable = callable;
			return qp;
		}

		public override IEnumerable Enumerable()
		{
			throw new NotSupportedException("SQL queries do not currently support enumeration");
		}

#if NET_2_0
		public override IEnumerable<T> Enumerable<T>()
		{
			throw new NotSupportedException("SQL queries do not currently support enumeration");
		}
#endif

		public ISQLQuery AddScalar(string columnAlias, IType type)
		{
			queryReturns.Add(new SQLQueryScalarReturn(columnAlias, type));
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
			queryReturns.Add(new SQLQueryJoinReturn(alias, ownerAlias, role, CollectionHelper.EmptyMap, lockMode));
			return this;
		}

		public ISQLQuery AddEntity(string alias, string entityName, LockMode lockMode)
		{
			queryReturns.Add(new SQLQueryRootReturn(alias, entityName, lockMode));
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
			ISQLQueryReturn[] returns = mapping.GetQueryReturns();
			int length = returns.Length;
			for (int index = 0; index < length; index++)
			{
				queryReturns.Add(returns[index]);
			}
			return this;
		}

		protected override void VerifyParameters()
		{
			base.VerifyParameters();
			bool noReturns = queryReturns==null || queryReturns.Count == 0;
			bool autodiscovertypes = false;
			if (noReturns)
			{
				autodiscovertypes = noReturns;
			}
			else
			{
				foreach (ISQLQueryReturn rtn in queryReturns)
				{
					if (rtn is SQLQueryScalarReturn)
					{
						SQLQueryScalarReturn scalar = (SQLQueryScalarReturn) rtn;
						if (scalar.Type == null)
						{
							autodiscovertypes = true;
							break;
						}
					}
				}
			}
			
			if (autodiscovertypes)
			{
				throw new QueryException("Return types of SQL query were not specified", QueryString);
			}
		}
	}
}

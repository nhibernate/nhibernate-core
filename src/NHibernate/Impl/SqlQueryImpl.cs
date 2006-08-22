using System;
using System.Collections;
#if NET_2_0
using System.Collections.Generic;
#endif

using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Type;
using NHibernate.Loader.Custom;

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
	public class SqlQueryImpl : AbstractQueryImpl
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
			ICollection querySpaces )
			: base( sql, session )
		{
			// TODO : this constructor form is *only* used from constructor directly below us; can it go away?
			queryReturns = new ArrayList( returnAliases.Length );
			for( int i = 0; i < returnAliases.Length; i++ )
			{
				SQLQueryRootReturn ret = new SQLQueryRootReturn(
					returnAliases[ i ],
					returnClasses[ i ].AssemblyQualifiedName,
					lockModes == null ? LockMode.None : lockModes[ i ] );
				queryReturns.Add( ret );
			}

			this.querySpaces = querySpaces;
			this.callable = false;
		}

		public SqlQueryImpl( string sql, string[] returnAliases, System.Type[] returnClasses, ISessionImplementor session, ICollection querySpaces )
			: this( sql, returnAliases, returnClasses, null, session, querySpaces )
		{
		}

		public SqlQueryImpl(NamedSQLQueryDefinition queryDef, ISessionImplementor session)
			: base(queryDef.QueryString, session)
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

		private ISQLQueryReturn[] GetQueryReturns()
		{
			ISQLQueryReturn[] result = new ISQLQueryReturn[ queryReturns.Count ];
			queryReturns.CopyTo( result, 0 );
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
			NativeSQLQuerySpecification spec = GenerateQuerySpecification( namedParams );
			QueryParameters qp = GetQueryParameters( namedParams );

			return Session.List( spec, qp );
		}

		public override void List( IList results )
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
			NativeSQLQuerySpecification spec = GenerateQuerySpecification( namedParams );
			QueryParameters qp = GetQueryParameters( namedParams );

			Session.List( spec, qp, results );
		}

#if NET_2_0
		public override IList<T> List<T>()
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
			NativeSQLQuerySpecification spec = GenerateQuerySpecification( namedParams );
			QueryParameters qp = GetQueryParameters( namedParams );

			return Session.List<T>( spec, qp );
		}
#endif

		public NativeSQLQuerySpecification GenerateQuerySpecification( IDictionary parameters )
		{
			return new NativeSQLQuerySpecification(
				BindParameterLists( NamedParams ),
				GetQueryReturns(),
				querySpaces );
		}

		public override QueryParameters GetQueryParameters(IDictionary namedParams)
		{
			QueryParameters qp = base.GetQueryParameters(namedParams);
			qp.Callable = callable;
			return qp;
		}
		
		public override IEnumerable Enumerable()
		{
			throw new NotSupportedException( "SQL queries do not currently support enumeration" );
		}

#if NET_2_0
		public override IEnumerable<T> Enumerable<T>()
		{
			throw new NotSupportedException( "SQL queries do not currently support enumeration" );
		}
#endif
	}
}

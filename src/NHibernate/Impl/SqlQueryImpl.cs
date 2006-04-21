using System;
using System.Collections;
using System.Collections.Generic;

using NHibernate.Engine;
using NHibernate.Type;

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
		private readonly System.Type[] returnClasses;
		private readonly string[] returnAliases;
		private readonly ICollection querySpaces;

		public SqlQueryImpl( string sql, string[] returnAliases, System.Type[] returnClasses, ISessionImplementor session, ICollection querySpaces) : base( sql, session )
		{
			this.returnClasses = returnClasses;
			this.returnAliases = returnAliases;
			this.querySpaces = querySpaces;
		}

		public string[] ReturnAliases
		{
			get { return returnAliases; }
		}

		public System.Type[] ReturnClasses
		{
			get { return returnClasses; }
		}

		public override IType[] ReturnTypes
		{
			get
			{
				IType[] types = new IType[ returnClasses.Length ];
				for ( int i = 0; i < returnClasses.Length; i++ )
				{
					types[ i ] = NHibernateUtil.Entity( returnClasses[ i ] );
				}

				return types;
			}
		}

		public override IList List()
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
			return Session.FindBySQL( BindParameterLists( namedParams ), returnAliases, returnClasses, GetQueryParameters( namedParams ), querySpaces );
		}

		public override IList<T> List<T>()
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
			return Session.FindBySQL<T>( BindParameterLists( namedParams ), returnAliases, returnClasses, GetQueryParameters( namedParams ), querySpaces );
		}
		
		public override IEnumerable Enumerable()
		{
			throw new NotSupportedException( "SQL queries do not currently support enumeration" );
		}

		public override IEnumerable<T> Enumerable<T>()
		{
			throw new NotSupportedException( "SQL queries do not currently support enumeration" );
		}
	}
}

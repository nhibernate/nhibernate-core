using System;
using System.Collections;
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
	internal class SqlQueryImpl : AbstractQueryImpl
	{
		private readonly System.Type[] returnClasses;
		private readonly string[] returnAliases;
		private readonly ICollection querySpaces;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="returnAliases"></param>
		/// <param name="returnClasses"></param>
		/// <param name="session"></param>
		/// <param name="querySpaces"></param>
		public SqlQueryImpl( string sql, string[] returnAliases, System.Type[] returnClasses, ISessionImplementor session, ICollection querySpaces) : base( sql, session )
		{
			this.returnClasses = returnClasses;
			this.returnAliases = returnAliases;
			this.querySpaces = querySpaces;
		}

		/// <summary>
		/// 
		/// </summary>
		public string[] ReturnAliases
		{
			get { return returnAliases; }
		}

		/// <summary>
		/// 
		/// </summary>
		public System.Type[] ReturnClasses
		{
			get { return returnClasses; }
		}

		/// <summary>
		/// 
		/// </summary>
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

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override IList List()
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
			return Session.FindBySQL( BindParameterLists( namedParams ), returnAliases, returnClasses, GetQueryParameters( namedParams ), querySpaces );
		}
		
		/// <summary></summary>
		public override IEnumerable Enumerable()
		{
			throw new NotSupportedException( "SQL queries do not currently support enumeration" );
		}
	}
}

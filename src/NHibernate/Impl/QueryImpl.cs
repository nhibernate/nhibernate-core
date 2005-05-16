using System;
using System.Collections;

using NHibernate.Engine;

namespace NHibernate.Impl
{
	internal class QueryImpl : AbstractQueryImpl
	{
		public QueryImpl( string queryString, ISessionImplementor session ) : base( queryString, session )
		{
		}
		
		/// <summary></summary>
		public override IEnumerable Enumerable()
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
			return Session.Enumerable( BindParameterLists( namedParams ), QueryParams( namedParams ) );
		}

		/// <summary></summary>
		public override IList List()
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
			return Session.Find( BindParameterLists( namedParams ), QueryParams( namedParams ) );
		}
	}
}
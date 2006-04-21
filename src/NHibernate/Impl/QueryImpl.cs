using System;
using System.Collections;
using System.Collections.Generic;

using NHibernate.Engine;

namespace NHibernate.Impl
{
	public class QueryImpl : AbstractQueryImpl
	{
		public QueryImpl( string queryString, ISessionImplementor session ) : base( queryString, session )
		{
		}
		
		public override IEnumerable Enumerable()
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
			return Session.Enumerable( BindParameterLists( namedParams ), GetQueryParameters( namedParams ) );
		}

		public override IEnumerable<T> Enumerable<T>()
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
			return Session.Enumerable<T>( BindParameterLists( namedParams ), GetQueryParameters( namedParams ) );
		}

		public override IList List()
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
			return Session.Find( BindParameterLists( namedParams ), GetQueryParameters( namedParams ) );
		}

		public override IList<T> List<T>()
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
			return Session.Find<T>( BindParameterLists( namedParams ), GetQueryParameters( namedParams ) );
		}
	}
}
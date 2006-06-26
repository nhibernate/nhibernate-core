using System;
using System.Collections;
#if NET_2_0
using System.Collections.Generic;
#endif

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

#if NET_2_0
		public override IEnumerable<T> Enumerable<T>()
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
			return Session.Enumerable<T>( BindParameterLists( namedParams ), GetQueryParameters( namedParams ) );
		}
#endif

		public override IList List()
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
			return Session.Find( BindParameterLists( namedParams ), GetQueryParameters( namedParams ) );
		}

		public override void List( IList results )
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
			Session.Find( BindParameterLists( namedParams ), GetQueryParameters( namedParams ), results );
		}

#if NET_2_0
		public override IList<T> List<T>()
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
			return Session.Find<T>( BindParameterLists( namedParams ), GetQueryParameters( namedParams ) );
		}
#endif
	}
}
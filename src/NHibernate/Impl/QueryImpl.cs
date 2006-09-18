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
		public QueryImpl(string queryString, FlushMode flushMode, ISessionImplementor session)
			: base(queryString, flushMode, session)
		{
		}

		public override IEnumerable Enumerable()
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
			Before();
			try
			{
				return Session.Enumerable(BindParameterLists(namedParams), GetQueryParameters(namedParams));
			}
			finally
			{
				After();
			}
		}

#if NET_2_0
		public override IEnumerable<T> Enumerable<T>()
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
			Before();
			try
			{
				return Session.Enumerable<T>(BindParameterLists(namedParams), GetQueryParameters(namedParams));
			}
			finally
			{
				After();
			}
		}
#endif

		public override IList List()
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
			Before();
			try
			{
				return Session.Find(BindParameterLists(namedParams), GetQueryParameters(namedParams));
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
			Before();
			try
			{
				Session.Find(BindParameterLists(namedParams), GetQueryParameters(namedParams), results);
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
			Before();
			try
			{
				return Session.Find<T>(BindParameterLists(namedParams), GetQueryParameters(namedParams));
			}
			finally
			{
				After();
			}
		}
#endif
	}
}
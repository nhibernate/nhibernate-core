using System;
using System.Collections;
using NHibernate.Engine;
using System.Collections.Generic;

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

		public override IList List()
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
			Before();
			try
			{
				return Session.List(BindParameterLists(namedParams), GetQueryParameters(namedParams));
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
				Session.List(BindParameterLists(namedParams), GetQueryParameters(namedParams), results);
			}
			finally
			{
				After();
			}
		}

		public override IList<T> List<T>()
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
			Before();
			try
			{
				return Session.List<T>(BindParameterLists(namedParams), GetQueryParameters(namedParams));
			}
			finally
			{
				After();
			}
		}
	}
}

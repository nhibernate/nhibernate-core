using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using System;

namespace NHibernate.Impl
{
	/// <summary> 
	/// Default implementation of the <see cref="IQuery"/>,
	/// for "ordinary" HQL queries (not collection filters)
	/// </summary>
	/// <seealso cref="CollectionFilterImpl"/>
	public class QueryImpl : AbstractQueryImpl
	{
		private readonly Dictionary<string, LockMode> lockModes = new Dictionary<string, LockMode>(2);

		public QueryImpl(string queryString, FlushMode flushMode, ISessionImplementor session, ParameterMetadata parameterMetadata)
			: base(queryString, flushMode, session, parameterMetadata)
		{
		}

		public QueryImpl(string queryString, ISessionImplementor session, ParameterMetadata parameterMetadata)
			: this(queryString, FlushMode.Unspecified, session, parameterMetadata)
		{
		}

		public override IEnumerable Enumerable()
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
			Before();
			try
			{
				return Session.Enumerable(ExpandParameterLists(namedParams), GetQueryParameters(namedParams));
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
				return Session.Enumerable<T>(ExpandParameterLists(namedParams), GetQueryParameters(namedParams));
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
				return Session.List(ExpandParameterLists(namedParams), GetQueryParameters(namedParams));
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
				Session.List(ExpandParameterLists(namedParams), GetQueryParameters(namedParams), results);
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
				return Session.List<T>(ExpandParameterLists(namedParams), GetQueryParameters(namedParams));
			}
			finally
			{
				After();
			}
		}

		public override IQuery SetLockMode(string alias, LockMode lockMode)
		{
			lockModes[alias] = lockMode;
			return this;
		}

		protected internal override IDictionary LockModes
		{
			get { return lockModes; }
		}

		public override int ExecuteUpdate()
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
			Before();
			try
			{
				return Session.ExecuteUpdate(ExpandParameterLists(namedParams), GetQueryParameters(namedParams));
			}
			finally
			{
				After();
			}
		}
	}
}

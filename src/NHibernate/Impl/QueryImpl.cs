using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Hql;

namespace NHibernate.Impl
{
	/// <summary> 
	/// Default implementation of the <see cref="IQuery"/>,
	/// for "ordinary" HQL queries (not collection filters)
	/// </summary>
	/// <seealso cref="CollectionFilterImpl"/>
	public class QueryImpl : AbstractQueryImpl2
	{
		public QueryImpl(string queryString, FlushMode flushMode, ISessionImplementor session, ParameterMetadata parameterMetadata)
			: base(queryString, flushMode, session, parameterMetadata)
		{
		}

		public QueryImpl(string queryString, ISessionImplementor session, ParameterMetadata parameterMetadata)
			: this(queryString, FlushMode.Unspecified, session, parameterMetadata)
		{
		}

		protected override IQueryExpression ExpandParameters(IDictionary<string, TypedValue> namedParams)
		{
			return ExpandParameterLists(namedParams).ToQueryExpression();
		}
	}
}

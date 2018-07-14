using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Impl;

namespace NHibernate.Multi
{
	public partial class QueryBatchItem<TResult> : QueryBatchItemBase<TResult>
	{
		protected readonly AbstractQueryImpl Query;

		public QueryBatchItem(IQuery query)
		{
			Query = (AbstractQueryImpl) query ?? throw new ArgumentNullException(nameof(query));
		}

		protected override List<QueryInfo> GetQueryInformation(ISessionImplementor session)
		{
			Query.VerifyParameters();
			QueryParameters queryParameters = Query.GetQueryParameters();
			queryParameters.ValidateParameters();

			return
				Query
					.GetTranslators(Session, queryParameters)
					.Select(t => new QueryInfo(queryParameters, t.Loader, new HashSet<string>(t.QuerySpaces), session))
					.ToList();
		}

		protected override IList<TResult> GetResultsNonBatched()
		{
			return Query.List<TResult>();
		}

		protected override List<TResult> DoGetResults()
		{
			return GetTypedResults<TResult>();
		}
	}
}

using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Loader.Criteria;
using NHibernate.Persister.Entity;

namespace NHibernate.Multi
{
	public partial class CriteriaBatchItem<T> : QueryBatchItemBase<T>
	{
		private readonly CriteriaImpl _criteria;

		public CriteriaBatchItem(ICriteria query)
		{
			_criteria = (CriteriaImpl) query ?? throw new ArgumentNullException(nameof(query));
		}

		protected override List<QueryInfo> GetQueryInformation(ISessionImplementor session)
		{
			var factory = Session.Factory;
			//for detached criteria
			if (_criteria.Session == null)
				_criteria.Session = Session;

			string[] implementors = factory.GetImplementors(_criteria.EntityOrClassName);
			int size = implementors.Length;
			var list = new List<QueryInfo>(size);
			for (int i = 0; i < size; i++)
			{
				CriteriaLoader loader = new CriteriaLoader(
					factory.GetEntityPersister(implementors[i]) as IOuterJoinLoadable,
					factory,
					_criteria,
					implementors[i],
					Session.EnabledFilters
				);

				list.Add(new QueryInfo(loader.Translator.GetQueryParameters(), loader, loader.QuerySpaces, session));
			}

			return list;
		}

		protected override IList<T> GetResultsNonBatched()
		{
			return _criteria.List<T>();
		}

		protected override List<T> DoGetResults()
		{
			return GetTypedResults<T>();
		}
	}
}

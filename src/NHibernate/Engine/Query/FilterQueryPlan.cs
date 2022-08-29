using System;
using System.Collections.Generic;

namespace NHibernate.Engine.Query
{
	/// <summary> 
	/// Extends an HQLQueryPlan to maintain a reference to the collection-role name
	/// being filtered. 
	/// </summary>
	[Serializable]
	public class FilterQueryPlan : QueryExpressionPlan
	{
		public FilterQueryPlan(IQueryExpression queryExpression, string collectionRole, bool shallow, IDictionary<string, IFilter> enabledFilters, ISessionFactoryImplementor factory)
			: base(queryExpression, collectionRole, shallow, enabledFilters, factory)
		{
			CollectionRole = collectionRole;
		}

		protected FilterQueryPlan(FilterQueryPlan source, IQueryExpression expression)
			: base(source, expression)
		{
			CollectionRole = source.CollectionRole;
		}

		public string CollectionRole { get; }

		public override QueryExpressionPlan Copy(IQueryExpression expression)
		{
			return new FilterQueryPlan(this, expression);
		}
	}
}

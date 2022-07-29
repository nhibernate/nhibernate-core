using System;
using System.Linq.Expressions;
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.ResultOperators
{
	public class NonAggregatingGroupBy : ClientSideTransformOperator
	{
		//Since v5.4
		[Obsolete("Please use another constructor")]
		public NonAggregatingGroupBy(GroupResultOperator groupBy) : this(groupBy, null)
		{
		}

		public NonAggregatingGroupBy(GroupResultOperator groupBy, Expression source)
		{
			GroupBy = groupBy;
			Source = source;
		}

		public GroupResultOperator GroupBy { get; }

		public Expression Source { get; }
	}
}

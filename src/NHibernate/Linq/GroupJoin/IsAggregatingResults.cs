using System.Collections.Generic;
using System.Linq.Expressions;
using Remotion.Linq.Clauses;

namespace NHibernate.Linq.GroupJoin
{
	public class IsAggregatingResults
	{
		public List<GroupJoinClause> NonAggregatingClauses  { get; set; }
		public List<GroupJoinClause> AggregatingClauses  { get; set; }
		public List<Expression> NonAggregatingExpressions { get; set; }
	}
}
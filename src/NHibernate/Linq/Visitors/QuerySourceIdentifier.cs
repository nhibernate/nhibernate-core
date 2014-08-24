using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// Identifies and names - using <see cref="QuerySourceNamer"/> - all QueryModel query sources
	/// </summary>
	/// <remarks>
	/// It may seem expensive to do this as a separate visitation of the query model, but unfortunately
	/// trying to identify query sources on the fly (i.e. while parsing the query model to generate
	/// the HQL expression tree) means a query source may be referenced by a <c>QuerySourceReference</c>
	/// before it has been identified - and named.
	/// </remarks>
	public class QuerySourceIdentifier : QueryModelVisitorBase
	{
		private readonly QuerySourceNamer _namer;

		private QuerySourceIdentifier(QuerySourceNamer namer)
		{
			_namer = namer;
		}

		public static void Visit(QuerySourceNamer namer, QueryModel queryModel)
		{
			new QuerySourceIdentifier(namer).VisitQueryModel(queryModel);
		}

		public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
		{
			_namer.Add(fromClause);
		}

		public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index)
		{
			_namer.Add(fromClause);
		}

		public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, GroupJoinClause groupJoinClause)
		{
			_namer.Add(joinClause);
		}

		public override void VisitGroupJoinClause(GroupJoinClause groupJoinClause, QueryModel queryModel, int index)
		{
			_namer.Add(groupJoinClause);
		}

		public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, int index)
		{
			_namer.Add(joinClause);
		}

		public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
		{
			var groupBy = resultOperator as GroupResultOperator;
			if (groupBy != null)
				_namer.Add(groupBy);
		}

		public QuerySourceNamer Namer { get { return _namer; } }
	}
}
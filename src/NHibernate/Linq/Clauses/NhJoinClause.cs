using System.Linq.Expressions;
using NHibernate.Linq.Visitors;
using Remotion.Linq.Clauses;

namespace NHibernate.Linq.Clauses
{
	/// <summary>
	/// All joins are created as outer joins. An optimization in <see cref="WhereJoinDetector"/> finds
	/// joins that may be inner joined and calls <see cref="MakeInner"/> on them.
	/// <see cref="QueryModelVisitor"/>'s <see cref="QueryModelVisitor.VisitAdditionalFromClause"/> will
	/// then emit the correct HQL join.
	/// </summary>
	public class NhJoinClause : AdditionalFromClause
	{
		public NhJoinClause(string itemName, System.Type itemType, Expression fromExpression) : base(itemName, itemType, fromExpression)
		{
			IsInner = false;
		}

		public bool IsInner { get; private set; }

		public static NhJoinClause Create(FromClauseBase fromClause)
		{
			return new NhJoinClause(fromClause.ItemName, fromClause.ItemType, fromClause.FromExpression);
		}

		public void MakeInner()
		{
			IsInner = true;
		}
	}
}
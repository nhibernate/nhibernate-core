using System;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace NHibernate.Linq.Clauses
{
	/// <summary>
	/// A wrapper for <see cref="JoinClause"/> that is used to mark it as an outer join.
	/// </summary>
	public class NhOuterJoinClause : NhClauseBase, IBodyClause, IClause, IQuerySource
	{
		public NhOuterJoinClause(JoinClause joinClause)
		{
			JoinClause = joinClause;
		}

		public JoinClause JoinClause { get; }

		public string ItemName => JoinClause.ItemName;

		public System.Type ItemType => JoinClause.ItemType;

		public void TransformExpressions(Func<Expression, Expression> transformation)
		{
			JoinClause.TransformExpressions(transformation);
		}

		public IBodyClause Clone(CloneContext cloneContext)
		{
			return new NhOuterJoinClause(JoinClause.Clone(cloneContext));
		}

		public override string ToString()
		{
			return $"outer {JoinClause}";
		}

		protected override void Accept(INhQueryModelVisitor visitor, QueryModel queryModel, int index)
		{
			if (visitor is INhQueryModelVisitorExtended queryModelVisitorExtended)
			{
				queryModelVisitorExtended.VisitNhOuterJoinClause(this, queryModel, index);
			}
			else
			{
				visitor.VisitJoinClause(JoinClause, queryModel, index);
			}
		}
	}
}

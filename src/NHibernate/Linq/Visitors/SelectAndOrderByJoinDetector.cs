using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Linq.ReWriters;
using Remotion.Linq.Clauses.Expressions;

namespace NHibernate.Linq.Visitors
{
	public class SelectAndOrderByJoinDetector : AbstractJoinDetector
	{
		internal SelectAndOrderByJoinDetector(NameGenerator nameGenerator, IIsEntityDecider isEntityDecider, Dictionary<string, NhJoinClause> joins, Dictionary<MemberExpression, QuerySourceReferenceExpression> expressionMap)
			: base(nameGenerator, isEntityDecider, joins, expressionMap)
		{
		}

		protected override Expression VisitMemberExpression(MemberExpression expression)
		{
			if (expression.Type.IsNonPrimitive() && _isEntityDecider.IsEntity(expression.Type))
			{
				return AddJoin(expression);
			}

			return base.VisitMemberExpression(expression);
		}
	}
}

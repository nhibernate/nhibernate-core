using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NHibernate.Util;
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq
{
	internal static class GroupResultOperatorExtensions
	{
		public static IEnumerable<Expression> ExtractKeyExpressions(this GroupResultOperator groupResult)
		{
			return groupResult.KeySelector.ExtractKeyExpressions();
		}

		private static IEnumerable<Expression> ExtractKeyExpressions(this Expression expr)
		{
			// Recursively extract key expressions from nested initializers 
			// --> new object[] { ((object)new object[] { x.A, x.B }), x.C }
			// --> new List<string> { x.A, x.B }
			// --> new CustomType(x.A, x.B) { C = x.C }
			if (expr is NewExpression newExpression)
				return newExpression.Arguments.SelectMany(ExtractKeyExpressions);
			if (expr is NewArrayExpression newArrayExpression)
				return newArrayExpression.Expressions.SelectMany(ExtractKeyExpressions);
			if (expr is MemberInitExpression memberInitExpression)
				return memberInitExpression.NewExpression.Arguments.SelectMany(ExtractKeyExpressions)
				                           .Union(memberInitExpression.Bindings.SelectMany(ExtractKeyExpressions));
			return new[] { expr };
		}

		private static IEnumerable<Expression> ExtractKeyExpressions(MemberBinding memberBinding)
		{
			switch (memberBinding)
			{
				case MemberAssignment memberAssignment:
					return memberAssignment.Expression.ExtractKeyExpressions();
				case MemberMemberBinding memberMemberBinding:
					return memberMemberBinding.Bindings.SelectMany(ExtractKeyExpressions);
				case MemberListBinding memberListBinding:
					return memberListBinding.Initializers.SelectMany(o => o.Arguments).SelectMany(ExtractKeyExpressions);
				default:
					return Enumerable.Empty<Expression>();
			}
		}
	}
}

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
			// --> x.A, x.B, x.C
			if (expr is NewExpression)
				return (expr as NewExpression).Arguments.SelectMany(ExtractKeyExpressions);
			if (expr is NewArrayExpression)
				return (expr as NewArrayExpression).Expressions.SelectMany(ExtractKeyExpressions);
			return new[] { expr };
		}
	}
}

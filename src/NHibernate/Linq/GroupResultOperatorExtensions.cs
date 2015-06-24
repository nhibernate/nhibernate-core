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
			if (groupResult.KeySelector is NewExpression)
				return (groupResult.KeySelector as NewExpression).Arguments;
			if (groupResult.KeySelector is NewArrayExpression)
				return (groupResult.KeySelector as NewArrayExpression).Expressions;
			return new [] { groupResult.KeySelector };
		}
	}
}

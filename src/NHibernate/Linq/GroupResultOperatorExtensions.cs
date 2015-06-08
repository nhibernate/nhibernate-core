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
		public static void ExtractKeyExpressions(this GroupResultOperator groupResult, IList<Expression> groupByKeys)
		{
			if (groupResult.KeySelector is NewExpression)
				(groupResult.KeySelector as NewExpression).Arguments.ForEach(groupByKeys.Add);
			else if (groupResult.KeySelector is NewArrayExpression)
				(groupResult.KeySelector as NewArrayExpression).Expressions.ForEach(groupByKeys.Add);
			else
				groupByKeys.Add(groupResult.KeySelector);
		}
	}
}

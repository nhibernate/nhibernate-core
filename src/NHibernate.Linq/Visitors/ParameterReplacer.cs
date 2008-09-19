using System.Collections.Generic;
using System.Linq.Expressions;

namespace NHibernate.Linq.Visitors
{
	public class ParameterReplacer : ExpressionVisitor
	{
		private readonly Dictionary<ParameterExpression, ParameterExpression> parameterReplacements;

		public ParameterReplacer(Dictionary<ParameterExpression, ParameterExpression> parameterReplacements)
		{
			this.parameterReplacements = parameterReplacements;
		}

		protected override Expression VisitParameter(ParameterExpression p)
		{
			if (parameterReplacements.ContainsKey(p))
				return parameterReplacements[p];
			return p;
		}
	}
}
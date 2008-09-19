using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NHibernate.Linq.Visitors
{
	public class ParameterReplacer : ExpressionVisitor
	{
		public ParameterReplacer(Dictionary<ParameterExpression, ParameterExpression> parameterReplacements)
		{
			this.parameterReplacements = parameterReplacements;
		}
		private Dictionary<ParameterExpression, ParameterExpression> parameterReplacements;

		protected override Expression VisitParameter(ParameterExpression p)
		{
			if (parameterReplacements.ContainsKey(p))
				return parameterReplacements[p];
			return p;
		}
	}
}

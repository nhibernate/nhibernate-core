using System.Collections.Generic;
using System.Linq.Expressions;

namespace NHibernate.Linq.Visitors
{
	public class Replacer : ExpressionVisitor
	{
		private readonly Dictionary<Expression, Expression> replacements;

		public Replacer(Dictionary<Expression, Expression> replacements)
		{
			this.replacements = replacements;
		}
		public override Expression Visit(Expression exp)
		{
			if (exp == null) 
				return exp;
			if (replacements.ContainsKey(exp))
				return replacements[exp];
			return base.Visit(exp);
		}
	}
}
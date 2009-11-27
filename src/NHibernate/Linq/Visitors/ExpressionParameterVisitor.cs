using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Data.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// Locates constants in the expression tree and generates parameters for each one
	/// </summary>
	public class ExpressionParameterVisitor : ExpressionTreeVisitor
	{
		private readonly Dictionary<ConstantExpression, NamedParameter> _parameters = new Dictionary<ConstantExpression, NamedParameter>();

		public static IDictionary<ConstantExpression, NamedParameter> Visit(Expression expression)
		{
			var visitor = new ExpressionParameterVisitor();
			
			visitor.VisitExpression(expression);

			return visitor._parameters;
		}

		protected override Expression VisitConstantExpression(ConstantExpression expression)
		{
			if (!typeof(IQueryable).IsAssignableFrom(expression.Type))
			{
				_parameters.Add(expression, new NamedParameter("p" + (_parameters.Count + 1), expression.Value, NHibernateUtil.GuessType(expression.Type)));
			}

			return base.VisitConstantExpression(expression);
		}
	}
}
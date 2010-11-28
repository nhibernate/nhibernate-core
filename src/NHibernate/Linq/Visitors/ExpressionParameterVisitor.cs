using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Type;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// Locates constants in the expression tree and generates parameters for each one
	/// </summary>
	public class ExpressionParameterVisitor : NhExpressionTreeVisitor
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
			if (!typeof(IQueryable).IsAssignableFrom(expression.Type) && !IsNullObject(expression))
			{
				// We use null for the type to indicate that the caller should let HQL figure it out.
				IType type = null;

				// We have a bit more information about the null parameter value.
				// Figure out a type so that HQL doesn't break on the null. (Related to NH-2430)
				if (expression.Value == null)
					type = NHibernateUtil.GuessType(expression.Type);

				// There is more information available in the Linq expression than to HQL directly.
				// In some cases it might be advantageous to use the extra info.  Assuming this
				// comes up, it would be nice to combine the HQL parameter type determination code
				// and the Expression information.

				_parameters.Add(expression, new NamedParameter("p" + (_parameters.Count + 1), expression.Value, type));
			}

			return base.VisitConstantExpression(expression);
		}

		private bool IsNullObject(ConstantExpression expression)
		{
			return expression.Type == typeof(Object) && expression.Value == null;
		}
	}
}
using System.Linq.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	internal class ConstantParametersRewriter : RelinqExpressionVisitor
	{
		private readonly VisitorParameters _parameters;

		public ConstantParametersRewriter(VisitorParameters parameters)
		{
			_parameters = parameters;
			Parameter = Expression.Parameter(typeof(object[]), "parameterValues");
		}

		public ConstantParametersRewriter(VisitorParameters parameters, ParameterExpression parameter)
		{
			_parameters = parameters;
			Parameter = parameter;
		}

		public ParameterExpression Parameter { get; }

		public static Expression Rewrite(Expression expression, VisitorParameters parameters, out ParameterExpression parameter)
		{
			var rewriter = new ConstantParametersRewriter(parameters);
			expression = rewriter.Visit(expression);
			parameter = rewriter.Parameter;
			return expression;
		}

		public static Expression Rewrite(Expression expression, VisitorParameters parameters, ParameterExpression parameter)
		{
			var rewriter = new ConstantParametersRewriter(parameters, parameter);
			expression = rewriter.Visit(expression);
			return expression;
		}

		protected override Expression VisitConstant(ConstantExpression expression)
		{
			if (_parameters.ConstantToParameterMap.TryGetValue(expression, out var namedParameter))
			{
				return Expression.Convert(
					Expression.ArrayIndex(Parameter, Expression.Constant(namedParameter.Index)),
					expression.Type);
			}

			return expression;
		}
	}
}

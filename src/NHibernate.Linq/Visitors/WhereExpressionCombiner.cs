using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using NHibernate.Linq.Util;

namespace NHibernate.Linq.Visitors
{
	public class WhereExpressionCombiner : ExpressionVisitor
	{

		protected override Expression VisitMethodCall(MethodCallExpression m)
		{
			if (IsWhereCall(m))
			{
				var stack = new Stack<Expression>();
				var parameterReplacements = new Dictionary<ParameterExpression, ParameterExpression>();

				var source = m.Arguments[0];
				var outerLambda = LinqUtil.StripQuotes(m.Arguments[1]) as LambdaExpression;
				var currentParameter = outerLambda.Parameters[0];
				var replacementParameter = Expression.Parameter(currentParameter.Type, GetParameterName());
				parameterReplacements[currentParameter] = replacementParameter;
				stack.Push(Visit(outerLambda.Body));
				while (source != null && IsWhereCall(source))
				{
					var mcall = source as MethodCallExpression;
					var innerLambda = LinqUtil.StripQuotes(mcall.Arguments[1]) as LambdaExpression;
					stack.Push(Visit(innerLambda.Body));
					currentParameter = innerLambda.Parameters[0];
					parameterReplacements[currentParameter] = replacementParameter;
					source = mcall.Arguments[0];
				}
				Expression expr = stack.Pop();
				foreach (var expression in stack)
				{
					expr = Expression.AndAlso(expr, expression);
				}
				source = Visit(source);
				var resultingLambda = Expression.Lambda(expr, replacementParameter);
				var parameterReplacer = new ParameterReplacer(parameterReplacements);
				resultingLambda = parameterReplacer.Visit(resultingLambda) as LambdaExpression;
				return Expression.Call(null, m.Method, source, resultingLambda);
			}
			else
				return base.VisitMethodCall(m);
		}



		protected static bool IsWhereCall(Expression ex)
		{
			if (ex is MethodCallExpression)
			{
				var m = ex as MethodCallExpression;
				var t = m.Method.DeclaringType;
				if (t == typeof(Enumerable) || t == typeof(Queryable))
				{
					if (m.Method.Name == "Where")
						return true;
				}
			}
			return false;
		}
		private string GetParameterName()
		{
			return string.Format("p_{0}", i++);

		}
		private int i = 0;

	}
}
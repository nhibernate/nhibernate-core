using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Param;
using NHibernate.Type;
using Remotion.Linq.Parsing;
using Remotion.Linq.Utilities;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// Locates constants in the expression tree and generates parameters for each one
	/// </summary>
	public class ExpressionParameterVisitor : ExpressionTreeVisitor
	{
		private readonly Dictionary<ConstantExpression, NamedParameter> _parameters = new Dictionary<ConstantExpression, NamedParameter>();
		private readonly ISessionFactoryImplementor _sessionFactory;

		private readonly ICollection<MethodBase> _pagingMethods = new HashSet<MethodBase>
			{
				ReflectionHelper.GetMethodDefinition(() => Queryable.Skip<object>(null, 0)),
				ReflectionHelper.GetMethodDefinition(() => Queryable.Take<object>(null, 0)),
				ReflectionHelper.GetMethodDefinition(() => Enumerable.Skip<object>(null, 0)),
				ReflectionHelper.GetMethodDefinition(() => Enumerable.Take<object>(null, 0)),
			};

		public ExpressionParameterVisitor(ISessionFactoryImplementor sessionFactory)
		{
			_sessionFactory = sessionFactory;
		}

		public static IDictionary<ConstantExpression, NamedParameter> Visit(Expression expression, ISessionFactoryImplementor sessionFactory)
		{
			var visitor = new ExpressionParameterVisitor(sessionFactory);
			
			visitor.VisitExpression(expression);

			return visitor._parameters;
		}

		protected override Expression VisitMethodCallExpression(MethodCallExpression expression)
		{
			var method = expression.Method.IsGenericMethod
							 ? expression.Method.GetGenericMethodDefinition()
							 : expression.Method;

			if (_pagingMethods.Contains(method) && !_sessionFactory.Dialect.SupportsVariableLimit)
			{
				//TODO: find a way to make this code cleaner
				var query = VisitExpression(expression.Arguments[0]);
				var arg = expression.Arguments[1];

				if (query == expression.Arguments[0])
					return expression;

				return Expression.Call(null, expression.Method, query, arg);
			}

			if (VisitorUtil.IsDynamicComponentDictionaryGetter(expression, _sessionFactory))
			{
				return expression;
			}

			return base.VisitMethodCallExpression(expression);
		}

		protected override Expression VisitConstantExpression(ConstantExpression expression)
		{
			if (!_parameters.ContainsKey(expression) && !typeof(IQueryable).IsAssignableFrom(expression.Type) && !IsNullObject(expression))
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

		private static bool IsNullObject(ConstantExpression expression)
		{
			return expression.Type == typeof(Object) && expression.Value == null;
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Param;
using NHibernate.Type;
using NHibernate.Util;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// Locates constants in the expression tree and generates parameters for each one
	/// </summary>
	public class ExpressionParameterVisitor : RelinqExpressionVisitor
	{
		private readonly Dictionary<ConstantExpression, NamedParameter> _parameters = new Dictionary<ConstantExpression, NamedParameter>();
		private readonly Dictionary<QueryVariable, NamedParameter> _variableParameters = new Dictionary<QueryVariable, NamedParameter>();
		private readonly IDictionary<ConstantExpression, QueryVariable> _queryVariables;
		private readonly ISessionFactoryImplementor _sessionFactory;
		private readonly QueryMode _queryMode;

		private static readonly MethodInfo QueryableSkipDefinition =
			ReflectHelper.FastGetMethodDefinition(Queryable.Skip, default(IQueryable<object>), 0);
		private static readonly MethodInfo QueryableTakeDefinition =
			ReflectHelper.FastGetMethodDefinition(Queryable.Take, default(IQueryable<object>), 0);
		private static readonly MethodInfo EnumerableSkipDefinition =
			ReflectHelper.FastGetMethodDefinition(Enumerable.Skip, default(IEnumerable<object>), 0);
		private static readonly MethodInfo EnumerableTakeDefinition =
			ReflectHelper.FastGetMethodDefinition(Enumerable.Take, default(IEnumerable<object>), 0);

		private readonly ICollection<MethodBase> _pagingMethods = new HashSet<MethodBase>
			{
				QueryableSkipDefinition, QueryableTakeDefinition,
				EnumerableSkipDefinition, EnumerableTakeDefinition
			};

		// Since v5.3
		[Obsolete("Please use overload with preTransformationResult and queryMode parameters instead.")]
		public ExpressionParameterVisitor(ISessionFactoryImplementor sessionFactory)
		{
			_sessionFactory = sessionFactory;
			// In order to keep the old behavior use a DML query mode to generate parameters for each constant
			_queryMode = QueryMode.Delete;
		}

		public ExpressionParameterVisitor(
			QueryMode queryMode,
			ISessionFactoryImplementor sessionFactory,
			PreTransformationResult preTransformationResult)
		{
			_queryMode = queryMode;
			_sessionFactory = sessionFactory;
			_queryVariables = preTransformationResult.QueryVariables;
		}

		// Since v5.3
		[Obsolete("Please use overload with preTransformationResult parameter instead.")]
		public static IDictionary<ConstantExpression, NamedParameter> Visit(Expression expression, ISessionFactoryImplementor sessionFactory)
		{
			var visitor = new ExpressionParameterVisitor(sessionFactory);
			visitor.Visit(expression);

			return visitor._parameters;
		}

		public static Expression Visit(
			QueryMode queryMode,
			PreTransformationResult preTransformationResult,
			ISessionFactoryImplementor sessionFactory,
			out IDictionary<ConstantExpression, NamedParameter> parameters)
		{
			var visitor = new ExpressionParameterVisitor(queryMode, sessionFactory, preTransformationResult);
			var expression = visitor.Visit(preTransformationResult.Expression);
			parameters = visitor._parameters;

			return expression;
		}

		protected override Expression VisitMethodCall(MethodCallExpression expression)
		{
			if (expression.Method.Name == nameof(LinqExtensionMethods.MappedAs) && expression.Method.DeclaringType == typeof(LinqExtensionMethods))
			{
				var rawParameter = Visit(expression.Arguments[0]);
				var parameter = rawParameter as ConstantExpression;
				var type = expression.Arguments[1] as ConstantExpression;
				if (parameter == null)
					throw new HibernateException(
						$"{nameof(LinqExtensionMethods.MappedAs)} must be called on an expression which can be evaluated as " +
						$"{nameof(ConstantExpression)}. It was call on {rawParameter?.GetType().Name ?? "null"} instead.");
				if (type == null)
					throw new HibernateException(
						$"{nameof(LinqExtensionMethods.MappedAs)} type must be supplied as {nameof(ConstantExpression)}. " +
						$"It was {expression.Arguments[1]?.GetType().Name ?? "null"} instead.");

				_parameters[parameter].Type = (IType)type.Value;

				return parameter;
			}

			var method = expression.Method.IsGenericMethod
							 ? expression.Method.GetGenericMethodDefinition()
							 : expression.Method;

			if (_pagingMethods.Contains(method) && !_sessionFactory.Dialect.SupportsVariableLimit)
			{
				//TODO: find a way to make this code cleaner
				var query = Visit(expression.Arguments[0]);
				var arg = expression.Arguments[1];

				if (query == expression.Arguments[0])
					return expression;

				return Expression.Call(null, expression.Method, query, arg);
			}

			if (VisitorUtil.IsDynamicComponentDictionaryGetter(expression, _sessionFactory))
			{
				return expression;
			}

			return base.VisitMethodCall(expression);
		}

		protected override Expression VisitConstant(ConstantExpression expression)
		{
			if (!_parameters.ContainsKey(expression) && !typeof(IQueryable).IsAssignableFrom(expression.Type) && !IsNullObject(expression))
			{
				// We use null for the type to indicate that the caller should let HQL figure it out.
				object value = expression.Value;
				IType type = null;

				// We have a bit more information about the null parameter value.
				// Figure out a type so that HQL doesn't break on the null. (Related to NH-2430)
				if (expression.Value == null)
					type = NHibernateUtil.GuessType(expression.Type);

				// Constant characters should be sent as strings
				if (expression.Type == typeof(char))
				{
					value = value.ToString();
				}

				// There is more information available in the Linq expression than to HQL directly.
				// In some cases it might be advantageous to use the extra info.  Assuming this
				// comes up, it would be nice to combine the HQL parameter type determination code
				// and the Expression information.

				// When QueryMode.Select, create only one parameter for the same variable. HQL does not support
				// reusing parameters for DML queries.
				NamedParameter parameter = null;
				if (_queryMode == QueryMode.Select &&
				    _queryVariables != null &&
				    _queryVariables.TryGetValue(expression, out var variable) &&
				    !_variableParameters.TryGetValue(variable, out parameter))
				{
					parameter = new NamedParameter("p" + (_parameters.Count + 1), value, type);
					_variableParameters.Add(variable, parameter);
				}

				if (parameter == null)
				{
					parameter = new NamedParameter("p" + (_parameters.Count + 1), value, type);
				}

				_parameters.Add(expression, parameter);

				return base.VisitConstant(expression);
			}

			return base.VisitConstant(expression);
		}

		private static bool IsNullObject(ConstantExpression expression)
		{
			return expression.Type == typeof(Object) && expression.Value == null;
		}
	}
}

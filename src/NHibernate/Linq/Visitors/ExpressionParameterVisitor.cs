using System;
using System.Collections;
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

		private static readonly ISet<MethodBase> PagingMethods = new HashSet<MethodBase>
		{
			ReflectionCache.EnumerableMethods.SkipDefinition,
			ReflectionCache.EnumerableMethods.TakeDefinition,
			ReflectionCache.QueryableMethods.SkipDefinition,
			ReflectionCache.QueryableMethods.TakeDefinition
		};

		// Since v5.3
		[Obsolete("Please use overload with preTransformationResult parameter instead.")]
		public ExpressionParameterVisitor(ISessionFactoryImplementor sessionFactory)
		{
			_sessionFactory = sessionFactory;
		}

		public ExpressionParameterVisitor(PreTransformationResult preTransformationResult)
		{
			_sessionFactory = preTransformationResult.SessionFactory;
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

		public static IDictionary<ConstantExpression, NamedParameter> Visit(PreTransformationResult preTransformationResult)
		{
			var visitor = new ExpressionParameterVisitor(preTransformationResult);
			visitor.Visit(preTransformationResult.Expression);
			return visitor._parameters;
		}

		protected override Expression VisitMethodCall(MethodCallExpression expression)
		{
			if (VisitorUtil.IsMappedAs(expression.Method))
			{
				var rawParameter = Visit(expression.Arguments[0]);
				// TODO 6.0: Remove below code and return expression as this logic is now inside ConstantTypeLocator
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

			if (PagingMethods.Contains(method) && !_sessionFactory.Dialect.SupportsVariableLimit)
			{
				var query = Visit(expression.Arguments[0]);
				//TODO 6.0: Remove the below code and return expression
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

#if NETCOREAPP2_0
		protected override Expression VisitInvocation(InvocationExpression expression)
		{
			if (ExpressionsHelper.TryGetDynamicMemberBinder(expression, out _))
			{
				// Avoid adding System.Runtime.CompilerServices.CallSite instance as a parameter
				base.Visit(expression.Arguments[1]);
				return expression;
			}

			return base.VisitInvocation(expression);
		}
#endif

		protected override Expression VisitConstant(ConstantExpression expression)
		{
			if (!_parameters.ContainsKey(expression) && !typeof(IQueryable).IsAssignableFrom(expression.Type) && !IsNullObject(expression))
			{
				// We use null for the type to indicate that the caller should let HQL figure it out.
				object value = expression.Value;
				IType type = null;

				// We have a bit more information about the null parameter value.
				// Figure out a type so that HQL doesn't break on the null. (Related to NH-2430)
				// In v5.3 types are calculated by ConstantTypeLocator, this logic is only for back compatibility.
				// TODO 6.0: Remove
				if (expression.Value == null)
					type = NHibernateUtil.GuessType(expression.Type);

				// Constant characters should be sent as strings
				// TODO 6.0: Remove
				if (_queryVariables == null && expression.Type == typeof(char))
				{
					value = value.ToString();
				}

				// There is more information available in the Linq expression than to HQL directly.
				// In some cases it might be advantageous to use the extra info.  Assuming this
				// comes up, it would be nice to combine the HQL parameter type determination code
				// and the Expression information.

				NamedParameter parameter = null;
				if (_queryVariables != null &&
				    _queryVariables.TryGetValue(expression, out var variable) &&
				    !_variableParameters.TryGetValue(variable, out parameter))
				{
					parameter = CreateParameter(expression, value, type);
					_variableParameters.Add(variable, parameter);
				}

				if (parameter == null)
				{
					parameter = CreateParameter(expression, value, type);
				}

				_parameters.Add(expression, parameter);

				return base.VisitConstant(expression);
			}

			return base.VisitConstant(expression);
		}

		private NamedParameter CreateParameter(ConstantExpression expression, object value, IType type)
		{
			var parameterName = "p" + (_parameters.Count + 1);
			return IsCollectionType(expression)
				? new NamedListParameter(parameterName, value, type)
				: new NamedParameter(parameterName, value, type);
		}

		private static bool IsNullObject(ConstantExpression expression)
		{
			return expression.Type == typeof(Object) && expression.Value == null;
		}

		private static bool IsCollectionType(ConstantExpression expression)
		{
			if (expression.Value != null)
			{
				return expression.Value is IEnumerable && !(expression.Value is string);
			}

			return expression.Type.IsCollectionType();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Param;
using NHibernate.Type;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// Locates constants in the expression tree and generates parameters for each one
	/// </summary>
	public class ExpressionParameterVisitor : ExpressionTreeVisitor
	{
		//NH-2401
		private readonly Dictionary<int, IType> parameterTypeOverrides = new Dictionary<int, IType>();
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
			return Visit(ref expression, sessionFactory);
		}

		internal static IDictionary<ConstantExpression, NamedParameter> Visit(ref Expression expression, ISessionFactoryImplementor sessionFactory)
		{
			var visitor = new ExpressionParameterVisitor(sessionFactory);

			expression = visitor.VisitExpression(expression);

			return visitor._parameters;
		}

		protected override Expression VisitMethodCallExpression(MethodCallExpression expression)
		{
			if (expression.Method.Name == "MappedAs" && expression.Method.DeclaringType == typeof(LinqExtensionMethods))
			{
				var parameter = (ConstantExpression)VisitExpression(expression.Arguments[0]);
				var type = (ConstantExpression)expression.Arguments[1];

				_parameters[parameter].Type = (IType)type.Value;

				return parameter;
			}

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

			/*if (expression.Method == ReflectionHelper.GetMethod<object>(x => x.ToString()))
			{
				//NH-2401: detect ToString after MappedAs
				//we just leave thos to StringGenerator
				return base.VisitMethodCallExpression(expression);
			}*/

			if ((expression.Method.DeclaringType == typeof(LinqExtensionMethods)) && (expression.Method.Name == "MappedAs"))
            {
				//NH-2401: detect MappedAs
				//we cannot do this in a *Generator class because there we don't have access to the parameters collection (_parameters)
                var typeExpression = Expression.Lambda<Func<IType>>(Expression.Convert(expression.Arguments[1], typeof(IType)));
                var type = typeExpression.Compile()();

				this.parameterTypeOverrides[_parameters.Count] = type;

				return expression;
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
				object value = expression.Value;
				IType type = null;

				//NH-2401
				if (this.parameterTypeOverrides.TryGetValue(this._parameters.Count, out type) == false)
				{
					// We have a bit more information about the null parameter value.
					// Figure out a type so that HQL doesn't break on the null. (Related to NH-2430)
					if (expression.Value == null)
						type = NHibernateUtil.GuessType(expression.Type);
				}

				// Constant characters should be sent as strings
				if (expression.Type == typeof(char))
				{
					value = value.ToString();
				}

				// There is more information available in the Linq expression than to HQL directly.
				// In some cases it might be advantageous to use the extra info.  Assuming this
				// comes up, it would be nice to combine the HQL parameter type determination code
				// and the Expression information.

				_parameters.Add(expression, new NamedParameter("p" + (_parameters.Count + 1), value, type));
			}

			return base.VisitConstantExpression(expression);
		}

		private static bool IsNullObject(ConstantExpression expression)
		{
			return expression.Type == typeof(Object) && expression.Value == null;
		}
	}
}

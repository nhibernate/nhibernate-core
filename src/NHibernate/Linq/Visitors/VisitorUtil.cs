using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Reflection;
using NHibernate.Util;
using Remotion.Linq.Parsing.ExpressionVisitors;
using NHibernate.Engine;
using System;

namespace NHibernate.Linq.Visitors
{
	public static class VisitorUtil
	{
		public static bool IsDynamicComponentDictionaryGetter(MethodInfo method, Expression targetObject, IEnumerable<Expression> arguments, ISessionFactory sessionFactory, out string memberName)
		{
			return TryGetPotentialDynamicComponentDictionaryMember(method, targetObject, arguments, out memberName)
				&& ExpressionsHelper.TryGetMappedType((ISessionFactoryImplementor) sessionFactory, targetObject, out var mappedType, out var _, out var _, out var _)
				// IDictionary can be mapped as collection or component - is it mapped as a component?
				&& mappedType.IsComponentType;
		}

		// Since 5.4
		[Obsolete("This method has no more usages and will be removed in a future version")]
		public static bool IsDynamicComponentDictionaryGetter(MethodCallExpression expression, ISessionFactory sessionFactory, out string memberName)
		{
			return IsDynamicComponentDictionaryGetter(expression.Method, expression.Object, expression.Arguments, sessionFactory, out memberName);
		}

		// Since 5.4
		[Obsolete("This method has no more usages and will be removed in a future version")]
		public static bool IsDynamicComponentDictionaryGetter(MethodCallExpression expression, ISessionFactory sessionFactory)
		{
			string memberName;
			return IsDynamicComponentDictionaryGetter(expression, sessionFactory, out memberName);
		}

		public static bool IsNullConstant(Expression expression)
		{
			var constantExpression = expression as ConstantExpression;
			return
				constantExpression != null &&
				constantExpression.Type.IsNullableOrReference() &&
				constantExpression.Value == null;
		}

		public static bool IsBooleanConstant(Expression expression, out bool value)
		{
			var constantExpr = expression as ConstantExpression;
			if (constantExpr != null && constantExpr.Type == typeof (bool))
			{
				value = (bool) constantExpr.Value;
				return true;
			}

			value = false; // Dummy value.
			return false;
		}

		/// <summary>
		/// Replaces a specific expression in an expression tree with a replacement expression.
		/// </summary>
		/// <param name="expression">The expression to search.</param>
		/// <param name="oldExpression">The expression to search for.</param>
		/// <param name="newExpression">The expression to replace with.</param>
		/// <returns></returns>
		public static Expression Replace(this Expression expression, Expression oldExpression, Expression newExpression)
		{
			return ReplacingExpressionVisitor.Replace(oldExpression, newExpression, expression);
		}

		/// <summary>
		/// Gets the member path.
		/// </summary>
		/// <param name="memberExpression">The member expression.</param>
		/// <returns></returns>
		public static string GetMemberPath(this MemberExpression memberExpression)
		{
			var path = memberExpression.Member.Name;
			var parentProp = memberExpression.Expression as MemberExpression;
			while (parentProp != null)
			{
				path = parentProp.Member.Name + "." + path;
				parentProp = parentProp.Expression as MemberExpression;
			}
			return path;
		}

		internal static bool TryGetPotentialDynamicComponentDictionaryMember(MethodCallExpression expression, out string memberName)
		{
			return TryGetPotentialDynamicComponentDictionaryMember(
				expression.Method,
				expression.Object,
				expression.Arguments,
				out memberName);
		}

		internal static bool TryGetPotentialDynamicComponentDictionaryMember(
			MethodInfo method,
			Expression targetObject,
			IEnumerable<Expression> arguments,
			out string memberName)
		{
			memberName = null;
			// A dynamic component must be an IDictionary with a string key.
			if (method.Name != "get_Item" ||
			    targetObject.NodeType != ExpressionType.MemberAccess || // Need the owning member (the dictionary).
			    !(arguments.First() is ConstantExpression key) ||
			    key.Type != typeof(string) ||
			    (!typeof(IDictionary).IsAssignableFrom(targetObject.Type) && !typeof(IDictionary<string, object>).IsAssignableFrom(targetObject.Type)))
			{
				return false;
			}

			// The potential member name
			memberName = (string) key.Value;
			return true;
		}

		internal static bool IsMappedAs(MethodInfo methodInfo)
		{
			return methodInfo.Name == nameof(LinqExtensionMethods.MappedAs) &&
			       methodInfo.DeclaringType == typeof(LinqExtensionMethods);
		}
	}
}

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Reflection;
using NHibernate.Util;
using Remotion.Linq.Clauses.Expressions;

namespace NHibernate.Linq.Visitors
{
	public static class VisitorUtil
	{
		public static bool IsDynamicComponentDictionaryGetter(MethodInfo method, Expression targetObject, IEnumerable<Expression> arguments, ISessionFactory sessionFactory, out string memberName)
		{
			memberName = null;

			// A dynamic component must be an IDictionary with a string key.

			if (method.Name != "get_Item" || !typeof(IDictionary).IsAssignableFrom(targetObject.Type))
				return false;

			var key = arguments.First() as ConstantExpression;
			if (key == null || key.Type != typeof(string))
				return false;

			// The potential member name
			memberName = (string)key.Value;

			// Need the owning member (the dictionary).
			var member = targetObject as MemberExpression;
			if (member == null)
				return false;

			var memberPath = member.Member.Name;
			var metaData = sessionFactory.GetClassMetadata(member.Expression.Type);

			//Walk backwards if the owning member is not a mapped class (i.e a possible Component)
			targetObject = member.Expression;
			while (metaData == null && targetObject != null &&
			       (targetObject.NodeType == ExpressionType.MemberAccess || targetObject.NodeType == ExpressionType.Parameter ||
			        targetObject.NodeType == QuerySourceReferenceExpression.ExpressionType))
			{
				System.Type memberType;
				if (targetObject.NodeType == QuerySourceReferenceExpression.ExpressionType)
				{
					var querySourceExpression = (QuerySourceReferenceExpression) targetObject;
					memberType = querySourceExpression.Type;
				}
				else if (targetObject.NodeType == ExpressionType.Parameter)
				{
					var parameterExpression = (ParameterExpression) targetObject;
					memberType = parameterExpression.Type;
				}
				else //targetObject.NodeType == ExpressionType.MemberAccess
				{
					var memberExpression = ((MemberExpression) targetObject);
					memberPath = memberExpression.Member.Name + "." + memberPath;
					memberType = memberExpression.Type;
					targetObject = memberExpression.Expression;
				}
				metaData = sessionFactory.GetClassMetadata(memberType);
			}

			if (metaData == null)
				return false;

			// IDictionary can be mapped as collection or component - is it mapped as a component?
			var propertyType = metaData.GetPropertyType(memberPath);
			return (propertyType != null && propertyType.IsComponentType);
		}

		public static bool IsDynamicComponentDictionaryGetter(MethodCallExpression expression, ISessionFactory sessionFactory, out string memberName)
		{
			return IsDynamicComponentDictionaryGetter(expression.Method, expression.Object, expression.Arguments, sessionFactory, out memberName);
		}

		public static bool IsDynamicComponentDictionaryGetter(MethodCallExpression expression, ISessionFactory sessionFactory)
		{
			string memberName;
			return IsDynamicComponentDictionaryGetter(expression, sessionFactory, out memberName);
		}


		public static bool IsNullConstant(Expression expression)
		{
			return expression is ConstantExpression &&
			       expression.Type.IsNullableOrReference() &&
			       ((ConstantExpression)expression).Value == null;
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
	}
}

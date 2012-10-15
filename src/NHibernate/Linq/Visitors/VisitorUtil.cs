using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Reflection;

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

			var metaData = sessionFactory.GetClassMetadata(member.Expression.Type);
			if (metaData == null)
				return false;

			// IDictionary can be mapped as collection or component - is it mapped as a component?
			var propertyType = metaData.GetPropertyType(member.Member.Name);
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
	}
}

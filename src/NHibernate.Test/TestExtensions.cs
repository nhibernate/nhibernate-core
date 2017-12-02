using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Test
{
	//Utility extension methods for use in unit tests. 
	public static class TestExtensions
	{
		public static T SetPropertyUsingReflection<T, TProp>(this T instance, Expression<Func<T, TProp>> property, TProp value)
		{
			var method = property.Body as MemberExpression;
			var propertyInfo = typeof(T).GetProperty(method.Member.Name);
			if (propertyInfo!= null && propertyInfo.CanWrite)
			{
				propertyInfo.SetValue(instance, value);
			}
			else
			{
				//camel cased field
				var name = method.Member.Name.Substring(0, 1).ToLowerInvariant() + method.Member.Name.Substring(1);
				var field = typeof(T).GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
				if (field != null)
				{
					field.SetValue(instance, value);
				}
			}
			return instance;
		}

	}
}

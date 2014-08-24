using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Context
{
	/// <summary>
	/// This class allows access to the HttpContext without referring to HttpContext at compile time.
	/// The accessors are cached as delegates for performance.
	/// </summary>
	public static class ReflectiveHttpContext
	{
		static ReflectiveHttpContext()
		{
			CreateCurrentHttpContextGetter();
			CreateHttpContextItemsGetter();
		}

		public static Func<object> HttpContextCurrentGetter { get; private set; }

		public static Func<object, IDictionary> HttpContextItemsGetter { get; private set; }

		public static IDictionary HttpContextCurrentItems
		{
			get { return HttpContextItemsGetter(HttpContextCurrentGetter()); }
		}

		private static System.Type HttpContextType
		{
			get
			{
				return
					System.Type.GetType(
						string.Format(
							"System.Web.HttpContext, System.Web, Version={0}, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
							Environment.Version));
			}
		}

		private static void CreateCurrentHttpContextGetter()
		{
			PropertyInfo currentProperty = HttpContextType.GetProperty("Current", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			Expression propertyExpression = Expression.Property(null, currentProperty);
			Expression convertedExpression = Expression.Convert(propertyExpression, typeof (object));
			HttpContextCurrentGetter = (Func<object>) Expression.Lambda(convertedExpression).Compile();
		}

		private static void CreateHttpContextItemsGetter()
		{
			ParameterExpression contextParam = Expression.Parameter(typeof (object), "context");
			Expression convertedParam = Expression.Convert(contextParam, HttpContextType);
			Expression itemsProperty = Expression.Property(convertedParam, "Items");
			HttpContextItemsGetter = (Func<object, IDictionary>) Expression.Lambda(itemsProperty, contextParam).Compile();
		}
	}
}
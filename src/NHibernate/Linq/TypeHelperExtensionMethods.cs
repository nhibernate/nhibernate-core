using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Linq
{
	public static class TypeHelperExtensionMethods
	{
		public static void ForEach<T>(this IEnumerable<T> query, Action<T> method)
		{
			foreach (T item in query)
			{
				method(item);
			}
		}

		public static bool IsEnumerableOfT(this System.Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
		}

		public static bool IsNullable(this System.Type type)
		{
			return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
		}

		public static bool IsNullableOrReference(this System.Type type)
		{
			return !type.IsValueType || type.IsNullable();
		}

		public static System.Type NullableOf(this System.Type type)
		{
			return type.GetGenericArguments()[0];
		}

		public static bool IsPrimitive(this System.Type type)
		{
			return (type.IsValueType || type.IsNullable() || type == typeof(string));
		}

		public static bool IsNonPrimitive(this System.Type type)
		{
			return !type.IsPrimitive();
		}


		/// <summary>
		/// Peform an unsafe cast to T (i.e. (T)source). This is only for
		/// syntactically more pleasing code.
		/// </summary>
		public static T As<T>(this object source)
		{
			return (T)source;
		}

		internal static bool IsCollectionType(this System.Type type)
		{
			return typeof (IEnumerable).IsAssignableFrom(type) && type != typeof (string);
		}
	}
}

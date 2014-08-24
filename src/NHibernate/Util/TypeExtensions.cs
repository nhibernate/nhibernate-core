using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Util
{
	public static class TypeExtensions
	{
		public static bool IsEnumerableOfT(this System.Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
		}

		public static bool IsNullable(this System.Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
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

		internal static bool IsCollectionType(this System.Type type)
		{
			return typeof (IEnumerable).IsAssignableFrom(type) && type != typeof (string);
		}

		internal static System.Type UnwrapIfNullable(this System.Type type)
		{
			if (type.IsNullable())
				return type.NullableOf();

			return type;
		}
	}
}

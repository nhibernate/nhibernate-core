using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Util
{
	public static class TypeExtensions
	{
		//Since 5.4
		[Obsolete("This method has no more usages and will be removed in a future version.")]
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
			return typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);
		}

		internal static System.Type UnwrapIfNullable(this System.Type type)
		{
			if (type.IsNullable())
				return type.NullableOf();

			return type;
		}

		internal static bool IsIntegralNumberType(this System.Type type)
		{
			var code = System.Type.GetTypeCode(type);
			if (code == TypeCode.SByte || code == TypeCode.Byte ||
				code == TypeCode.Int16 || code == TypeCode.UInt16 ||
				code == TypeCode.Int32 || code == TypeCode.UInt32 ||
				code == TypeCode.Int64 || code == TypeCode.UInt64)
			{
				return true;
			}

			return false;
		}

		internal static bool IsRealNumberType(this System.Type type)
		{
			var code = System.Type.GetTypeCode(type);
			if (code == TypeCode.Decimal || code == TypeCode.Single || code == TypeCode.Double)
			{
				return true;
			}

			return false;
		}
	}
}

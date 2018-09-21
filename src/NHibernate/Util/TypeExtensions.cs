using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NHibernate.Util
{
	public static class TypeExtensions
	{
		private const BindingFlags PropertiesOrFieldOfClass = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

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

		internal static IEnumerable<MemberInfo> GetFieldsOfHierarchy(this System.Type type)
		{
			if (!type.IsInterface)
			{
				var analyzing = type;
				while (analyzing != null && analyzing != typeof(object))
				{
					foreach (var fieldInfo in GetUserDeclaredFields(analyzing))
					{
						yield return fieldInfo;
					}

					analyzing = analyzing.BaseType;
				}
			}
		}

		internal static IEnumerable<PropertyInfo> GetPropertiesOfHierarchy(this System.Type type)
		{
			if (!type.IsInterface)
			{
				var analyzing = type;
				while (analyzing != null && analyzing != typeof(object))
				{
					foreach (var fieldInfo in analyzing.GetProperties(PropertiesOrFieldOfClass))
					{
						yield return fieldInfo;
					}

					analyzing = analyzing.BaseType;
				}
			}
		}

		internal static IEnumerable<FieldInfo> GetUserDeclaredFields(this System.Type type)
		{
			// Anonymous types are implemented with properties having backing fields not flagged
			// with the compiler generated attribute. Under C# their name starts with a '<', under
			// VB with a '$'. Excluding names starting with '<' also excludes C# auto-properties
			// backing fields, but not VB's ones, which start with an '_'. We cannot filter out
			// on the underscore character, since it is a valid starting character for regular
			// field names. But these fields have the compiler generated attribute. So we
			// have to exclude names starting with '<' or '$', and fields having the compiler
			// generated attribute.
			// If we were not caring about anonymous types, filtering on the compiler generated
			// attribute would be enough for both C# and VB (and likely other languages). I have
			// not seen why it is needed to care for anonymous types with current usages of this
			// method.
			return 
				type
					.GetFields(PropertiesOrFieldOfClass)
					.Where(x => !x.Name.StartsWith('<') && !x.Name.StartsWith('$') &&
						       x.CustomAttributes.All(a => a.AttributeType != typeof(CompilerGeneratedAttribute)));
		}
	}
}

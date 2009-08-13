// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Linq
{
	/// <summary>
	/// Type related helper methods
	/// </summary>
	public static class TypeHelper
	{
		public static System.Type FindIEnumerable(System.Type seqType)
		{
			if (seqType == null || seqType == typeof (string))
				return null;
			if (seqType.IsArray)
				return typeof (IEnumerable<>).MakeGenericType(seqType.GetElementType());
			if (seqType.IsGenericType)
			{
				foreach (System.Type arg in seqType.GetGenericArguments())
				{
					System.Type ienum = typeof (IEnumerable<>).MakeGenericType(arg);
					if (ienum.IsAssignableFrom(seqType))
					{
						return ienum;
					}
				}
			}
			System.Type[] ifaces = seqType.GetInterfaces();
			if (ifaces != null && ifaces.Length > 0)
			{
				foreach (System.Type iface in ifaces)
				{
					System.Type ienum = FindIEnumerable(iface);
					if (ienum != null) return ienum;
				}
			}
			if (seqType.BaseType != null && seqType.BaseType != typeof (object))
			{
				return FindIEnumerable(seqType.BaseType);
			}
			return null;
		}

		public static System.Type GetSequenceType(System.Type elementType)
		{
			return typeof (IEnumerable<>).MakeGenericType(elementType);
		}

		public static System.Type GetElementType(System.Type seqType)
		{
			System.Type ienum = FindIEnumerable(seqType);
			if (ienum == null) return seqType;
			return ienum.GetGenericArguments()[0];
		}

		public static bool IsNullableType(System.Type type)
		{
			return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);
		}

		public static bool IsNullAssignable(System.Type type)
		{
			return !type.IsValueType || IsNullableType(type);
		}

		public static System.Type GetNonNullableType(System.Type type)
		{
			if (IsNullableType(type))
			{
				return type.GetGenericArguments()[0];
			}
			return type;
		}

		public static System.Type GetNullAssignableType(System.Type type)
		{
			if (!IsNullAssignable(type))
			{
				return typeof (Nullable<>).MakeGenericType(type);
			}
			return type;
		}

		public static ConstantExpression GetNullConstant(System.Type type)
		{
			return Expression.Constant(null, GetNullAssignableType(type));
		}

		public static System.Type GetMemberType(MemberInfo mi)
		{
			var fi = mi as FieldInfo;
			if (fi != null) return fi.FieldType;
			var pi = mi as PropertyInfo;
			if (pi != null) return pi.PropertyType;
			var ei = mi as EventInfo;
			if (ei != null) return ei.EventHandlerType;
			return null;
		}

		public static object GetDefault(System.Type type)
		{
			bool isNullable = !type.IsValueType || IsNullableType(type);
			if (!isNullable)
				return Activator.CreateInstance(type);
			return null;
		}

		public static bool IsReadOnly(MemberInfo member)
		{
			switch (member.MemberType)
			{
				case MemberTypes.Field:
					return (((FieldInfo) member).Attributes & FieldAttributes.InitOnly) != 0;
				case MemberTypes.Property:
					var pi = (PropertyInfo) member;
					return !pi.CanWrite || pi.GetSetMethod() == null;
				default:
					return true;
			}
		}
	}
}
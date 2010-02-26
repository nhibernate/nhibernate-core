using System;
using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Linq
{
	public static class LinqExtensionMethods
	{
        public static IQueryable<T> Query<T>(this ISession session)
        {
            return new NhQueryable<T>(session);
        }

        public static void ForEach<T>(this IEnumerable<T> query, Action<T> method)
        {
            foreach (T item in query)
            {
                method(item);
            }
        }

        public static bool IsEnumerableOfT(this System.Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (IEnumerable<>);
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
            return (type.IsValueType || type.IsNullable() || type == typeof (string));
        }

        public static bool IsNonPrimitive(this System.Type type)
        {
            return !type.IsPrimitive();
        }

        public static T As<T>(this object source)
        {
            return (T) source;
        }
    }
}
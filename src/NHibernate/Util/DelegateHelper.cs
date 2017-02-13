using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Util
{
	internal static class DelegateHelper
	{
		public static Func<object, T> BuildPropertyGetter<T>(System.Type type, string propertyName)
		{
			var parameter = Expression.Parameter(typeof (object), "x");
			var instance = Expression.Convert(parameter, type);
			var property = Expression.Property(instance, propertyName);
			return Expression.Lambda<Func<object, T>>(property, parameter).Compile();
		}

		public static Action<object, T> BuildPropertySetter<T>(System.Type type, string propertyName)
		{
			var parameter = Expression.Parameter(typeof(object), "x");
			var instance = Expression.Convert(parameter, type);
			var property = Expression.Property(instance, propertyName);
			var value = Expression.Parameter(typeof (T), "value");
			var assign = Expression.Assign(property, value);
			return Expression.Lambda<Action<object, T>>(assign, parameter, value).Compile();
		}

		public static Action<object> BuildAction(System.Type type, string methodName)
		{
			var parameter = Expression.Parameter(typeof (object));
			var instance = Expression.Convert(parameter, type);
			var methodCall = Expression.Call(
				instance,
				GetMethod(type, methodName));

			return Expression.Lambda<Action<object>>(methodCall, parameter).Compile();
		}

		public static Action<object, T> BuildAction<T>(System.Type type, string methodName)
		{
			var parameter = Expression.Parameter(typeof (object), "x");
			var instance = Expression.Convert(parameter, type);

			var arg0 = Expression.Parameter(typeof (T), "arg0");

			var methodCall = Expression.Call(
				instance,
				GetMethod(type, methodName, typeof (T)),
				arg0);

			return Expression.Lambda<Action<object, T>>(methodCall, parameter, arg0).Compile();
		}

		public static Action<object, T1, T2> BuildAction<T1, T2>(System.Type type, string methodName)
		{
			var parameter = Expression.Parameter(typeof (object), "x");
			var instance = Expression.Convert(parameter, type);

			var arg0 = Expression.Parameter(typeof (T1), "arg0");
			var arg1 = Expression.Parameter(typeof (T2), "arg1");

			var methodCall = Expression.Call(
				instance,
				GetMethod(type, methodName, typeof (T1), typeof (T2)),
				arg0,
				arg1);

			return Expression.Lambda<Action<object, T1, T2>>(methodCall, parameter, arg0, arg1).Compile();
		}

		public static Func<object, TReturn> BuildFunc<TReturn>(System.Type type, string methodName)
		{
			var parameter = Expression.Parameter(typeof (object));
			var instance = Expression.Convert(parameter, type);
			var methodCall = Expression.Call(
				instance,
				GetMethod(type, methodName));

			return Expression.Lambda<Func<object, TReturn>>(methodCall, parameter).Compile();
		}

		private static MethodInfo GetMethod(System.Type type, string methodName, params System.Type[] types)
		{
			return type.GetMethod(
				methodName,
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
				null,
				types,
				null);
		}
	}
}

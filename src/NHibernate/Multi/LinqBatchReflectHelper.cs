using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Util;

namespace NHibernate.Multi
{
	static class LinqBatchReflectHelper
	{
		private static readonly ConcurrentDictionary<System.Type, Func<ILinqBatchItem, IList>> GetResultsForTypeDic = new ConcurrentDictionary<System.Type, Func<ILinqBatchItem, IList>>();

		private static readonly MethodInfo GetTypedResultsMethod = ReflectHelper.GetMethod((ILinqBatchItem i) => i.GetTypedResults<object>()).GetGenericMethodDefinition();

		internal static IList GetTypedResults(ILinqBatchItem batchItem, System.Type type)
		{
			return GetResultsForTypeDic.GetOrAdd(type, t => CompileDelegate(t)).Invoke(batchItem);
		}

		private static Func<ILinqBatchItem, IList> CompileDelegate(System.Type type)
		{
			var generic = GetTypedResultsMethod.MakeGenericMethod(type);
			var instance = Expression.Parameter(typeof(ILinqBatchItem));
			var methodCall = Expression.Call(instance, generic);
			return Expression.Lambda<Func<ILinqBatchItem, IList>>(methodCall, instance).Compile();
		}
	}
}

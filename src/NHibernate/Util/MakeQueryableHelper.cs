using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace NHibernate.Util
{
	internal static class MakeQueryableHelper
	{
		private static readonly MethodInfo _implMethodDefinition =
			ReflectHelper.GetMethodDefinition(() => MakeQueryableImpl<object>(null));

		public static object MakeQueryable(IEnumerable enumerable, System.Type elementType)
		{
			return _implMethodDefinition.MakeGenericMethod(elementType).Invoke(null, new object[] { enumerable });
		}

		private static object MakeQueryableImpl<TElement>(IEnumerable enumerable)
		{
			return enumerable.Cast<TElement>().AsQueryable();
		}
	}
}

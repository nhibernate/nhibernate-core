using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace NHibernate.Util
{
	internal static class MakeQueryableHelper
    {
		private static readonly Lazy<MethodInfo> _implMethod = new Lazy<MethodInfo>(() =>
			typeof(MakeQueryableHelper).GetMethod(nameof(MakeQueryableImpl), BindingFlags.Static | BindingFlags.NonPublic));

		public static object MakeQueryable(IEnumerable enumerable, System.Type elementType)
		{
			return _implMethod.Value.MakeGenericMethod(elementType).Invoke(null, new object[] { enumerable });
		}

		private static object MakeQueryableImpl<TElement>(IEnumerable enumerable)
		{
			return enumerable.Cast<TElement>().AsQueryable();
		}
	}
}

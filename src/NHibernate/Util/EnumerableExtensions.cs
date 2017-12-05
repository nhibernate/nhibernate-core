using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Util
{
	//Since v5.1
	[Obsolete("This class has no more usages and will be removed in next major version.")]
	public static class EnumerableExtensions
	{
		//Since v5.1
		[Obsolete("Please use Enumerable.Any<T>(IEnumerable<T>) instead.")]
		public static bool Any(this IEnumerable source)
		{
			return Enumerable.Any(source.Cast<object>());
		}

		//Since v5.1
		[Obsolete("Please use Enumerable.First<T>(IEnumerable<T>) instead.")]
		public static object First(this IEnumerable source)
		{
			return Enumerable.First(source.Cast<object>());
		}

		//Since v5.1
		[Obsolete("Please use Enumerable.FirstOrDefault<T>(IEnumerable<T>) instead.")]
		public static object FirstOrNull(this IEnumerable source)
		{
			return Enumerable.FirstOrDefault(source.Cast<object>());
		}

		//Since v5.1
		[Obsolete("Please use a loop instead.")]
		public static void ForEach<T>(this IEnumerable<T> query, Action<T> method)
		{
			foreach (var item in query)
			{
				method(item);
			}
		}
	}
}

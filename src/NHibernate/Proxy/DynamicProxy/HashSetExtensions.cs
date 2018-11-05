using System;
using System.Collections.Generic;

namespace NHibernate.Proxy.DynamicProxy
{
	[Obsolete("This class is not used anymore and will be removed in a next major version")]
	public static class HashSetExtensions
	{
		[Obsolete("This method is not used anymore and will be removed in a next major version")]
		public static HashSet<T> Merge<T>(this HashSet<T> source, IEnumerable<T> toMerge)
		{
			foreach (T item in toMerge)
			{
				source.Add(item);
			}
			return source;
		}
	}
}

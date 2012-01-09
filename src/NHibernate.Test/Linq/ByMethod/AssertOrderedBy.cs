using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.Linq.ByMethod
{
	public static class AssertOrderedBy
	{
		public static void Ascending<T, TProperty>(IList<T> result, Func<T, TProperty> keySelector)
			where TProperty : IComparable
		{
			if (result == null) throw new ArgumentNullException("result");
			for (int i = 0; i < result.Count - 1; i++)
			{
				var first = result[i];
				var second = result[i + 1];
				Assert.LessOrEqual(keySelector(first), keySelector(second));
			}
		}
		
		public static void Descending<T, TProperty>(IList<T> result, Func<T, TProperty> keySelector)
			where TProperty : IComparable
		{
			if (result == null) throw new ArgumentNullException("result");
			for (int i = 0; i < result.Count - 1; i++)
			{
				var first = result[i];
				var second = result[i + 1];
				Assert.GreaterOrEqual(keySelector(first), keySelector(second));
			}
		}
	}
}
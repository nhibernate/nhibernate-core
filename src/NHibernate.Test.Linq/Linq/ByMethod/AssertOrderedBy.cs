using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.Linq.ByMethod
{
	public static class AssertOrderedBy
	{
		public static void Ascending<T>(IList<T> result, Func<T, IComparable> keySelector)
		{
			if (result == null) throw new ArgumentNullException("result");
			for (var i = 0; i < result.Count - 1; i++)
			{
				var first = result[i];
				var second = result[i + 1];
				Assert.That(keySelector(first), Is.LessThanOrEqualTo(keySelector(second)));
			}
		}

		public static void Descending<T>(IList<T> result, Func<T, IComparable> keySelector)
		{
			if (result == null) throw new ArgumentNullException("result");
			for (var i = 0; i < result.Count - 1; i++)
			{
				var first = result[i];
				var second = result[i + 1];
				Assert.That(keySelector(first), Is.GreaterThanOrEqualTo(keySelector(second)));
			}
		}
	}
}
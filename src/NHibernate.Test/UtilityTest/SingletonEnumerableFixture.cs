using System.Collections;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	[TestFixture]
	public class SingletonEnumerableFixture
	{
		[Test]
		public void DifferentEnumeratorInEachRequest()
		{
			var obj = new object();
			var se = new SingletonEnumerable<object>(obj);
			Assert.That(!ReferenceEquals(se.GetEnumerator(), se.GetEnumerator()));

			// with no generic enumerator
			var see = (IEnumerable) se;
			Assert.That(!ReferenceEquals(see.GetEnumerator(), see.GetEnumerator()));
		}

		[Test]
		public void ShouldWorkInForeach()
		{
			var obj = new object();
			var se = new SingletonEnumerable<object>(obj);
			int i=0;
			foreach (var o in se)
			{
				i++;
			}
			Assert.That(i, Is.EqualTo(1));
		}

		[Test]
		public void ShouldWorkAsEnumerator()
		{
			var obj = new object();
			var se = new SingletonEnumerable<object>(obj);
			var enu = se.GetEnumerator();
			int i = 0;
			while (enu.MoveNext())
			{
				i++;
			}
			Assert.That(i, Is.EqualTo(1));
		}
	}
}
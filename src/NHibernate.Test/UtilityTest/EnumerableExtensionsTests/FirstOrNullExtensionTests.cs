using System;
using System.Collections;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest.EnumerableExtensionsTests
{
	public class FirstOrNullExtensionTests
	{
		[Test]
		public void WhenNullThenThenThrows()
		{
			Assert.That(() => ((IEnumerable)null).FirstOrNull(), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public void WhenHasElementsThenReturnFirst()
		{
			Assert.That((new[] { 2, 1 }).FirstOrNull(), Is.EqualTo(2));
		}

		[Test]
		public void WhenEmptyThenReturnNull()
		{
			Assert.That((new object[0]).FirstOrNull(), Is.Null);
		}
	}
}
using System;
using System.Collections;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest.EnumerableExtensionsTests
{
	[TestFixture]
	public class FirstOrNullExtensionTests
	{
		[Test]
		public void WhenNullThenThenThrows()
		{
#pragma warning disable 618 // FirstOrNull is obsolete
			Assert.That(() => ((IEnumerable)null).FirstOrNull(), Throws.TypeOf<ArgumentNullException>());
#pragma warning restore 618
		}

		[Test]
		public void WhenHasElementsThenReturnFirst()
		{
#pragma warning disable 618 // FirstOrNull is obsolete
			Assert.That((new[] { 2, 1 }).FirstOrNull(), Is.EqualTo(2));
#pragma warning restore 618
		}

		[Test]
		public void WhenEmptyThenReturnNull()
		{
#pragma warning disable 618 // FirstOrNull is obsolete
			Assert.That((new object[0]).FirstOrNull(), Is.Null);
#pragma warning restore 618
		}
	}
}

using System;
using System.Collections;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest.EnumerableExtensionsTests
{
	[TestFixture]
	public class FirstExtensionTests
	{
		[Test]
		public void WhenNullThenThenThrows()
		{
#pragma warning disable CS0618 // Type or member is obsolete
			Assert.That(() => ((IEnumerable)null).First(), Throws.TypeOf<ArgumentNullException>());
#pragma warning restore CS0618 // Type or member is obsolete
		}

		[Test]
		public void WhenHasElementsThenReturnFirst()
		{
#pragma warning disable CS0618 // Type or member is obsolete
			Assert.That((new[] { 2, 1 }).First(), Is.EqualTo(2));
#pragma warning restore CS0618 // Type or member is obsolete
		}

		[Test]
		public void WhenEmptyThenThrowsInvalidOperation()
		{
#pragma warning disable CS0618 // Type or member is obsolete
			Assert.That(() => (new object[0]).First(), Throws.TypeOf<InvalidOperationException>());
#pragma warning restore CS0618 // Type or member is obsolete
		}
	}
}

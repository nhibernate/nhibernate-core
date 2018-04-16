using System;
using System.Collections;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest.EnumerableExtensionsTests
{
	//Since v5.1
	[Obsolete]
	[TestFixture]
	public class FirstExtensionTests
	{
		[Test]
		public void WhenNullThenThenThrows()
		{
			Assert.That(() => ((IEnumerable)null).First(), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public void WhenHasElementsThenReturnFirst()
		{
			Assert.That((new[] { 2, 1 }).First(), Is.EqualTo(2));
		}

		[Test]
		public void WhenEmptyThenThrowsInvalidOperation()
		{
			Assert.That(() => (Array.Empty<object>()).First(), Throws.TypeOf<InvalidOperationException>());
		}
	}
}

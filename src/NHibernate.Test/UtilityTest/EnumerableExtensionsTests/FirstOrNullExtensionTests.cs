using System;
using System.Collections;
using NHibernate.Util;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.UtilityTest.EnumerableExtensionsTests
{
	public class FirstOrNullExtensionTests
	{
		[Test]
		public void WhenNullThenThenThrows()
		{
			Executing.This(() => ((IEnumerable)null).FirstOrNull()).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenHasElementsThenReturnFirst()
		{
			(new[] { 2, 1 }).FirstOrNull().Should().Be(2);
		}

		[Test]
		public void WhenEmptyThenReturnNull()
		{
			(new object[0]).FirstOrNull().Should().Be.Null();
		}
	}
}
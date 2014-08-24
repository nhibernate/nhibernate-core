using System;
using System.Collections;
using NHibernate.Util;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.UtilityTest.EnumerableExtensionsTests
{
	public class FirstExtensionTests
	{
		[Test]
		public void WhenNullThenThenThrows()
		{
			Executing.This(() => ((IEnumerable)null).First()).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenHasElementsThenReturnFirst()
		{
			(new[] { 2, 1 }).First().Should().Be(2);
		}

		[Test]
		public void WhenEmptyThenThrowsInvalidOperation()
		{
			Executing.This(() => (new object[0]).First()).Should().Throw<InvalidOperationException>();
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	[TestFixture]
	public class ArrayHelperTests
	{
		[Test]
		public void GetHashCodeShouldBeEqual()
		{
			var a = new[] {1, 2, 3, 4};
			var b = new[] {1, 2, 3, 4};

			Assert.That(ArrayHelper.ArrayGetHashCode(a), Is.EqualTo(ArrayHelper.ArrayGetHashCode(b)));
		}
	}
}

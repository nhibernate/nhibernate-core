using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	[TestFixture]
	public class ArrayHelperTests
	{
		[Test]
		public void NullArraysShouldBeEqual()
		{
			bool[] a = null, b = null;
			Assert.That(ArrayHelper.ArrayEquals(a, b), Is.True);
		}

		[Test]
		public void EitherArrayNullShouldNotBeEqual()
		{
			bool[] a = new[] { true, false, true }, b = null;

			Assert.That(ArrayHelper.ArrayEquals(a, b), Is.False);
			Assert.That(ArrayHelper.ArrayEquals(b, a), Is.False);
		}

		[Test]
		public void ArraysShouldBeEqual()
		{
			var a = new[] { 1, 2, 3, 4 };
			var b = new[] { 1, 2, 3, 4 };

			Assert.That(ArrayHelper.ArrayEquals(a, b), Is.True);
			Assert.That(ArrayHelper.ArrayEquals(b, a), Is.True);
		}
	}
}

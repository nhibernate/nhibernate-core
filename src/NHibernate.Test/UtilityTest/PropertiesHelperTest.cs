using System.Collections.Generic;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	public class PropertiesHelperTest
	{
		[Test]
		public void WhenInvalidBoolValueThenUseDefault()
		{
			Assert.That(PropertiesHelper.GetBoolean("myProp", new Dictionary<string, string> {{"myProp", "pizza"}}, false), Is.False);
		}

		[Test]
		public void WhenInvalidInt32ValueThenUseDefault()
		{
			Assert.That(PropertiesHelper.GetInt32("myProp", new Dictionary<string, string> { { "myProp", "pizza" } }, 5), Is.EqualTo(5));
		}

		[Test]
		public void WhenInvalidInt64ValueThenUseDefault()
		{
			Assert.That(PropertiesHelper.GetInt64("myProp", new Dictionary<string, string> { { "myProp", "pizza" } }, 5), Is.EqualTo(5));
		}

		[Test]
		public void WhenValidBoolValueThenValue()
		{
			Assert.That(PropertiesHelper.GetBoolean("myProp", new Dictionary<string, string> { { "myProp", "true" } }, false), Is.True);
		}

		[Test]
		public void WhenValidInt32ValueThenValue()
		{
			Assert.That(PropertiesHelper.GetInt32("myProp", new Dictionary<string, string> { { "myProp", int.MaxValue.ToString() } }, 5), Is.EqualTo(int.MaxValue));
		}

		[Test]
		public void WhenValidInt64ValueThenValue()
		{
			Assert.That(PropertiesHelper.GetInt64("myProp", new Dictionary<string, string> { { "myProp", long.MaxValue.ToString() } }, 5), Is.EqualTo(long.MaxValue));
		}
	}
}
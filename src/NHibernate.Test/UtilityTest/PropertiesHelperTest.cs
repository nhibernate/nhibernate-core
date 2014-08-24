using System.Collections.Generic;
using NHibernate.Util;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.UtilityTest
{
	public class PropertiesHelperTest
	{
		[Test]
		public void WhenInvalidBoolValueThenUseDefault()
		{
			PropertiesHelper.GetBoolean("myProp", new Dictionary<string, string> {{"myProp", "pizza"}}, false).Should().Be.False();
		}

		[Test]
		public void WhenInvalidInt32ValueThenUseDefault()
		{
			PropertiesHelper.GetInt32("myProp", new Dictionary<string, string> { { "myProp", "pizza" } }, 5).Should().Be(5);
		}

		[Test]
		public void WhenInvalidInt64ValueThenUseDefault()
		{
			PropertiesHelper.GetInt64("myProp", new Dictionary<string, string> { { "myProp", "pizza" } }, 5).Should().Be(5);
		}

		[Test]
		public void WhenValidBoolValueThenValue()
		{
			PropertiesHelper.GetBoolean("myProp", new Dictionary<string, string> { { "myProp", "true" } }, false).Should().Be.True();
		}

		[Test]
		public void WhenValidInt32ValueThenValue()
		{
			PropertiesHelper.GetInt32("myProp", new Dictionary<string, string> { { "myProp", int.MaxValue.ToString() } }, 5).Should().Be(int.MaxValue);
		}

		[Test]
		public void WhenValidInt64ValueThenValue()
		{
			PropertiesHelper.GetInt64("myProp", new Dictionary<string, string> { { "myProp", long.MaxValue.ToString() } }, 5).Should().Be(long.MaxValue);
		}
	}
}
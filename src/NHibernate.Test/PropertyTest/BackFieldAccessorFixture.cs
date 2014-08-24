using NHibernate.Properties;
using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	[TestFixture]
	public class BackFieldAccessorFixture
	{
		[Test]
		public void GetValue()
		{
			var accessor = PropertyAccessorFactory.GetPropertyAccessor("backfield");
			var getter = accessor.GetGetter(typeof(MyAutoProp), "AutoProp");
			var rogetter = accessor.GetGetter(typeof(MyAutoProp), "ReadOnlyAutoProp");

			Assert.That(getter.Get(new MyAutoProp { AutoProp = -1 }), Is.EqualTo(-1));
			Assert.That(getter.Get(new MyAutoProp { AutoProp = 1 }), Is.EqualTo(1));

			Assert.That(rogetter.Get(new MyAutoProp()), Is.EqualTo(0));
			Assert.That(rogetter.Get(new MyAutoProp(5)), Is.EqualTo(5));
		}

		[Test]
		public void SetValue()
		{
			var accessor = PropertyAccessorFactory.GetPropertyAccessor("backfield");
			var getter = accessor.GetGetter(typeof(MyAutoProp), "AutoProp");
			var setter = accessor.GetSetter(typeof(MyAutoProp), "AutoProp");

			var rogetter = accessor.GetGetter(typeof(MyAutoProp), "ReadOnlyAutoProp");
			var rosetter = accessor.GetSetter(typeof(MyAutoProp), "ReadOnlyAutoProp");

			var i = new MyAutoProp { AutoProp = -1 };
			Assert.That(getter.Get(i), Is.EqualTo(-1));
			setter.Set(i, 5);
			Assert.That(getter.Get(i), Is.EqualTo(5));

			Assert.That(rogetter.Get(new MyAutoProp()), Is.EqualTo(0));
			rosetter.Set(i, 123);
			Assert.That(rogetter.Get(i), Is.EqualTo(123));
		}
	}

	public class MyAutoProp
	{
		public MyAutoProp() {}

		public MyAutoProp(int readOnlyAutoProp)
		{
			ReadOnlyAutoProp = readOnlyAutoProp;
		}

		public int AutoProp { get; set; }
		public int ReadOnlyAutoProp { get; private set; }
	}
}
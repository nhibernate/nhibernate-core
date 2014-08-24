using NHibernate.Properties;
using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	[TestFixture]
	public class ReadonlyAccessorFixture
	{
		[Test]
		public void GetValue()
		{
			var accessor = PropertyAccessorFactory.GetPropertyAccessor("readonly");
			var getter = accessor.GetGetter(typeof(Calculation), "Sum");

			Assert.That(getter.Get(new Calculation()), Is.EqualTo(2));
		}

		[Test]
		public void SetValue()
		{
			var accessor = PropertyAccessorFactory.GetPropertyAccessor("readonly");
			var getter = accessor.GetGetter(typeof(Calculation), "Sum");
			var setter = accessor.GetSetter(typeof(Calculation), "Sum");

			var i = new Calculation();
			Assert.That(getter.Get(i), Is.EqualTo(2));
			setter.Set(i, 1);
			Assert.That(getter.Get(i), Is.EqualTo(2));
		}
	}

	public class Calculation
	{
		public Calculation() { }

		public int Sum { get { return 1 + 1; } }
	}
}

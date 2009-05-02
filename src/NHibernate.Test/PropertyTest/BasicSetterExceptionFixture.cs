using System;
using NHibernate.Properties;
using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	/// <summary>
	/// Test the exception messages that come out a BasicSetter when
	/// invalid values are passed in.
	/// </summary>
	[TestFixture]
	public class BasicSetterExceptionFixture
	{
		protected IPropertyAccessor _accessor;
		protected ISetter _setter;

		[SetUp]
		public void SetUp()
		{
			_accessor = PropertyAccessorFactory.GetPropertyAccessor("property");
			_setter = _accessor.GetSetter(typeof(A), "Id");
		}

		[Test]
		public void SetInvalidType()
		{
			A instance = new A();
			var e = Assert.Throws<PropertyAccessException>(() => _setter.Set(instance, "wrong type"));
			Assert.That(e.Message, Is.EqualTo("The type System.String can not be assigned to a property of type System.Int32 setter of NHibernate.Test.PropertyTest.BasicSetterExceptionFixture+A.Id"));
		}

		[Test]
		public void SetValueArgumentException()
		{
			A instance = new A();
			// this will throw a TargetInvocationException that gets wrapped in a PropertyAccessException
			var e = Assert.Throws<PropertyAccessException>(() => _setter.Set(instance, 5));
			Assert.That(e.Message, Is.EqualTo("could not set a property value by reflection setter of NHibernate.Test.PropertyTest.BasicSetterExceptionFixture+A.Id"));
		}

		public class A
		{
			private int _id = 0;

			public int Id
			{
				get { return _id; }
				set
				{
					if (value == 5)
					{
						throw new ArgumentException("can't be 5 for testing purposes");
					}
					_id = value;
				}
			}
		}
	}
}
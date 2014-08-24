using System;
using NHibernate.Properties;
using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	[TestFixture]
	public class NoSetterCamelCaseMUnderscoreFixture : NoSetterAccessorFixture
	{
		[SetUp]
		public override void SetUp()
		{
			_expectedCamelMUnderscoreGetterCalled = true;

			_accessor = PropertyAccessorFactory.GetPropertyAccessor("nosetter.camelcase-m-underscore");
			_getter = _accessor.GetGetter(typeof(FieldClass), "CamelMUnderscore");
			_setter = _accessor.GetSetter(typeof(FieldClass), "CamelMUnderscore");
			_instance = new FieldClass();
			_instance.InitCamelCaseMUnderscore(0);
		}
	}
}

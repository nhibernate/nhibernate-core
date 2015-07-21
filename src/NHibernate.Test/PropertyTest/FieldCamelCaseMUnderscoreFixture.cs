using System;
using NHibernate.Properties;
using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	[TestFixture]
	public class FieldCamelCaseMUnderscoreFixture : FieldAccessorFixture
	{
		[SetUp]
		public override void SetUp()
		{
			_accessor = PropertyAccessorFactory.GetPropertyAccessor("field.camelcase-m-underscore");
			_getter = _accessor.GetGetter(typeof(FieldClass), "CamelMUnderscore");
			_setter = _accessor.GetSetter(typeof(FieldClass), "CamelMUnderscore");
			_instance = new FieldClass();
			_instance.InitCamelCaseMUnderscore(0);
		}
	}
}

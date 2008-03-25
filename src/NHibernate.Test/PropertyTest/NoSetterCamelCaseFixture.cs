using System;
using NHibernate.Properties;
using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	/// <summary>
	/// Summary description for NoSetterCamelCaseFixture.
	/// </summary>
	[TestFixture]
	public class NoSetterCamelCaseFixture : NoSetterAccessorFixture
	{
		[SetUp]
		public override void SetUp()
		{
			_expectedCamelBazGetterCalled = true;
			_accessor = PropertyAccessorFactory.GetPropertyAccessor("nosetter.camelcase");
			_getter = _accessor.GetGetter(typeof(FieldClass), "CamelBaz");
			_setter = _accessor.GetSetter(typeof(FieldClass), "CamelBaz");
			_instance = new FieldClass();
			_instance.InitCamelBaz(0);
		}
	}
}
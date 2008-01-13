using System;
using NHibernate.Properties;
using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	/// <summary>
	/// Summary description for FieldCamelCaseFixture.
	/// </summary>
	[TestFixture]
	public class FieldCamelCaseFixture : FieldAccessorFixture
	{
		[SetUp]
		public override void SetUp()
		{
			_accessor = PropertyAccessorFactory.GetPropertyAccessor("field.camelcase");
			_getter = _accessor.GetGetter(typeof(FieldClass), "CamelBaz");
			_setter = _accessor.GetSetter(typeof(FieldClass), "CamelBaz");
			_instance = new FieldClass();
			_instance.InitCamelBaz(0);
		}
	}
}
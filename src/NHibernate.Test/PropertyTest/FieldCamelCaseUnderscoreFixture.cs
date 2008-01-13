using System;
using NHibernate.Properties;
using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	/// <summary>
	/// Summary description for FieldCamelCaseUnderscoreFixture.
	/// </summary>
	[TestFixture]
	public class FieldCamelCaseUnderscoreFixture : FieldAccessorFixture
	{
		[SetUp]
		public override void SetUp()
		{
			_accessor = PropertyAccessorFactory.GetPropertyAccessor("field.camelcase-underscore");
			_getter = _accessor.GetGetter(typeof(FieldClass), "CamelUnderscoreFoo");
			_setter = _accessor.GetSetter(typeof(FieldClass), "CamelUnderscoreFoo");
			_instance = new FieldClass();
			_instance.InitCamelUnderscoreFoo(0);
		}
	}
}
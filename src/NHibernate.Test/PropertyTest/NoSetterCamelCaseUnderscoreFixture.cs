using System;
using NHibernate.Properties;
using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	/// <summary>
	/// Summary description for NoSetterCamelCaseUnderscoreFixture.
	/// </summary>
	[TestFixture]
	public class NoSetterCamelCaseUnderscoreFixture : NoSetterAccessorFixture
	{
		[SetUp]
		public override void SetUp()
		{
			_expectedCamelUnderscoreFooGetterCalled = true;

			_accessor = PropertyAccessorFactory.GetPropertyAccessor("nosetter.camelcase-underscore");
			_getter = _accessor.GetGetter(typeof(FieldClass), "CamelUnderscoreFoo");
			_setter = _accessor.GetSetter(typeof(FieldClass), "CamelUnderscoreFoo");
			_instance = new FieldClass();
			_instance.InitCamelUnderscoreFoo(0);
		}
	}
}
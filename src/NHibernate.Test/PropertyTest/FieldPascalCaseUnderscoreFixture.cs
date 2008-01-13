using System;
using NHibernate.Properties;
using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	/// <summary>
	/// Test the <c>field.pascalcase-underscore</c> access strategy.
	/// </summary>
	[TestFixture]
	public class FieldPascalCaseUnderscoreFixture : FieldAccessorFixture
	{
		[SetUp]
		public override void SetUp()
		{
			_accessor = PropertyAccessorFactory.GetPropertyAccessor("field.pascalcase-underscore");
			_getter = _accessor.GetGetter(typeof(FieldClass), "PascalUnderscoreFoo");
			_setter = _accessor.GetSetter(typeof(FieldClass), "PascalUnderscoreFoo");
			_instance = new FieldClass();
			_instance.InitPascalUnderscoreFoo(0);
		}
	}
}
using System;
using NHibernate.Property;
using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	/// <summary>
	/// Test the <c>nosetter.pascalcase-underscore</c> access strategy.
	/// </summary>
	public class NoSetterPascalCaseUnderscoreFixture : NoSetterAccessorFixture
	{
		[SetUp]
		public override void SetUp() 
		{
			_expectedPascalUnderscoreFooGetterCalled = true;
	
			_accessor = PropertyAccessorFactory.GetPropertyAccessor("nosetter.pascalcase-underscore");
			_getter = _accessor.GetGetter( typeof(FieldClass), "PascalUnderscoreFoo" );
			_setter = _accessor.GetSetter( typeof(FieldClass), "PascalUnderscoreFoo" );
			_instance = new FieldClass();
			_instance.InitPascalUnderscoreFoo( 0 );
		}
	}
}

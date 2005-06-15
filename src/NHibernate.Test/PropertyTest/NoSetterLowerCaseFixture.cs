using System;
using NHibernate.Property;
using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	/// <summary>
	/// Test the <c>nosetter.lowercase</c> access strategy.
	/// </summary>
	[TestFixture]
	public class NoSetterLowerCaseFixture : NoSetterAccessorFixture
	{
		[SetUp]
		public override void SetUp() 
		{
			_expectedLowerFooGetterCalled = true;
	
			_accessor = PropertyAccessorFactory.GetPropertyAccessor("nosetter.lowercase");
			_getter = _accessor.GetGetter( typeof(FieldClass), "LowerFoo" );
			_setter = _accessor.GetSetter( typeof(FieldClass), "LowerFoo" );
			_instance = new FieldClass();
			_instance.InitLowerFoo( 0 );
		}
	}
}

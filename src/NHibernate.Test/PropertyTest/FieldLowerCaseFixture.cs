using System;
using System;
using NHibernate.Property;
using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	/// <summary>
	/// Test the <c>field.lowercase</c> access strategy.
	/// </summary>
	[TestFixture]
	public class FieldLowerCaseFixture : FieldAccessorFixture
	{
		[SetUp]
		public override void SetUp() 
		{
			_accessor = PropertyAccessorFactory.GetPropertyAccessor("field.lowercase");
			_getter = _accessor.GetGetter( typeof(FieldClass), "LowerFoo" );
			_setter = _accessor.GetSetter( typeof(FieldClass), "LowerFoo" );
			_instance = new FieldClass();
			_instance.InitLowerFoo( 0 );
		}
		
	}
}

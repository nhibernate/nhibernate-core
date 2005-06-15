using System;

using NHibernate.Property;

using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	/// <summary>
	/// Test the <c>field.pascalcase-m-underscore</c> access strategy.
	/// </summary>
	[TestFixture]
	public class FieldPascalCaseMUnderscoreFixture : FieldAccessorFixture
	{
		[SetUp]
		public override void SetUp() 
		{
			_accessor = PropertyAccessorFactory.GetPropertyAccessor("field.pascalcase-m-underscore");
			_getter = _accessor.GetGetter( typeof(FieldClass), "Blah" );
			_setter = _accessor.GetSetter( typeof(FieldClass), "Blah" );
			_instance = new FieldClass();
			_instance.InitBlah( 0 );
		}
	}
}

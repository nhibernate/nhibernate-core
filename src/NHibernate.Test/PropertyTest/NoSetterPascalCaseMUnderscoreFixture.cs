using System;

using NHibernate.Property;

using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	/// <summary>
	/// Summary description for NoSetterCamelCaseUnderscoreFixture.
	/// </summary>
	[TestFixture]
	public class NoSetterPascalCaseMUnderscoreFixture : NoSetterAccessorFixture
	{
		[SetUp]
		public override void SetUp()
		{
			_expectedBlahGetterCalled = true;
	
			_accessor = PropertyAccessorFactory.GetPropertyAccessor("nosetter.pascalcase-m-underscore");
			_getter = _accessor.GetGetter( typeof(FieldClass), "Blah" );
			_setter = _accessor.GetSetter( typeof(FieldClass), "Blah" );
			_instance = new FieldClass( 6, -1, 0, 2 );
			
		}
	
	}
}

using System;

using NHibernate.Property;

using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	/// <summary>
	/// Summary description for NoSetterLowerCaseUnderscoreFixture.
	/// </summary>
	[TestFixture]
	public class NoSetterLowerCaseUnderscoreFixture : NoSetterAccessorFixture
	{
		[SetUp]
		public override void SetUp()
		{
			_expectedLowerUnderscoreFooGetterCalled = true;
	
			_accessor = PropertyAccessorFactory.GetPropertyAccessor("nosetter.lowercase-underscore");
			_getter = _accessor.GetGetter( typeof(FieldClass), "LowerUnderscoreFoo" );
			_setter = _accessor.GetSetter( typeof(FieldClass), "LowerUnderscoreFoo" );
			_instance = new FieldClass();
			_instance.InitLowerUnderscoreFoo( 0 );
		}
	
	}
}


using System;

using NHibernate.Property;

using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	/// <summary>
	/// Summary description for FieldLowerCaseUnderscoreFixture.
	/// </summary>
	[TestFixture]
	public class FieldLowerCaseUnderscoreFixture : FieldAccessorFixture
	{
		[SetUp]
		public override void SetUp() 
		{
			_accessor = PropertyAccessorFactory.GetPropertyAccessor("field.lowercase-underscore");
			_getter = _accessor.GetGetter( typeof(FieldClass), "LowerUnderscoreFoo" );
			_setter = _accessor.GetSetter( typeof(FieldClass), "LowerUnderscoreFoo" );
			_instance = new FieldClass();
			_instance.InitLowerUnderscoreFoo( 0 );
		}
	}
}

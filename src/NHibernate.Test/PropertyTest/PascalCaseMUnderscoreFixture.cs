using System;

using NHibernate.Property;

using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	/// <summary>
	/// Summary description for PascalCaseMUnderscoreFixture.
	/// </summary>
	[TestFixture]
	public class PascalCaseMUnderscoreFixture : FieldAccessorFixture
	{
		[SetUp]
		public override void SetUp() 
		{
			_accessor = PropertyAccessorFactory.GetPropertyAccessor("field.pascalcase-m-underscore");
			_getter = _accessor.GetGetter( typeof(FieldClass), "Blah" );
			_setter = _accessor.GetSetter( typeof(FieldClass), "Blah" );
			_instance = new FieldClass( 6, -12 , 0, 13);
		}
	}
}

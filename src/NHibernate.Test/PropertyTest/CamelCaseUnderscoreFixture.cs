using System;

using NHibernate.Property;

using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	/// <summary>
	/// Summary description for CamelCaseUnderscoreFixture.
	/// </summary>
	[TestFixture]
	public class CamelCaseUnderscoreFixture : FieldAccessorFixture
	{
		[SetUp]
		public override void SetUp() 
		{
			_accessor = PropertyAccessorFactory.GetPropertyAccessor("field.camelcase-underscore");
			_getter = _accessor.GetGetter( typeof(FieldClass), "Id" );
			_setter = _accessor.GetSetter( typeof(FieldClass), "Id" );
			_instance = new FieldClass(2, 0 , -4, 3 );
		}
	}
}

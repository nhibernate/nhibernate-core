using System;

using NHibernate.Property;

using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	/// <summary>
	/// Summary description for CamelCaseFixture.
	/// </summary>
	[TestFixture]
	public class CamelCaseFixture : FieldAccessorFixture
	{
		[SetUp]
		public override void SetUp() 
		{
			_accessor = PropertyAccessorFactory.GetPropertyAccessor("field.camelcase");
			_getter = _accessor.GetGetter( typeof(FieldClass), "CamelBaz" );
			_setter = _accessor.GetSetter( typeof(FieldClass), "CamelBaz" );
			_instance = new FieldClass(2, -4, 3, 0 );
		}
	}
}


using System;

using NHibernate.Property;

using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	/// <summary>
	/// Summary description for FieldUnderscorePrefixAccessorFixture.
	/// </summary>
	[TestFixture]
	public class FieldUnderscorePrefixAccessorFixture : FieldAccessorFixture
	{
		[SetUp]
		public override void SetUp() 
		{
			_accessor = new FieldUnderscorePrefixAccessor();
			_getter = _accessor.GetGetter( typeof(FieldClass), "Id" );
			_setter = _accessor.GetSetter( typeof(FieldClass), "Id" );
			_instance = new FieldClass( 0 , -4, 3 );
		}
	}
}

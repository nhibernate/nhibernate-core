using System;

using NHibernate.Property;

using NUnit.Framework;

namespace NHibernate.Test.PropertyTest
{
	/// <summary>
	/// Summary description for FieldMUnderscorePrefixAccessorFixture.
	/// </summary>
	[TestFixture]
	public class FieldMUnderscorePrefixAccessorFixture : FieldAccessorFixture
	{
		[SetUp]
		public override void SetUp() 
		{
			_accessor = new FieldMUnderscorePrefixAccessor();
			_getter = _accessor.GetGetter( typeof(FieldClass), "Id" );
			_setter = _accessor.GetSetter( typeof(FieldClass), "Id" );
			_instance = new FieldClass( -12 , 0, 13);
		}
	}
}

using System;

using NHibernate.Type;

using NUnit.Framework;

namespace NHibernate.Test.TypesTest 
{

	/// <summary>
	/// TestFixtures for the <see cref="DateTimeType"/>.
	/// </summary>
	[TestFixture]
	public class DateTimeTypeFixture 
	{
		[Test]
		public void DeepCopyNotNull() 
		{
			NullableType type = NHibernate.DateTime;

			object value1 = DateTime.Now;

			object value2 = type.DeepCopyNotNull(value1);

			Assert.AreEqual( value1, value2, "Copies should be the same.");
			Assert.IsFalse( Object.ReferenceEquals(value1, value2), "Should be different objects in memory." );

			value2 = ((DateTime)value2).AddHours(2);

			Assert.IsFalse( value1==value2, "value2 was changed, should not be the same." );
		}
	}
}

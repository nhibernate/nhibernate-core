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
			

			value2 = ((DateTime)value2).AddHours(2);
			Assert.IsFalse( value1==value2, "value2 was changed, value1 should not have changed also." );
		}
	}
}

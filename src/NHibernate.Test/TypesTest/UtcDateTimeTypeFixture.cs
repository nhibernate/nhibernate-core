using System;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// The Unit Tests for the UtcDateTimeType.
	/// </summary>
	[TestFixture]
	public class UtcDateTimeTypeFixture : TypeFixtureBase
	{
		protected override string TypeName => "DateTime";

		[Test]
		public void ReadWrite()
		{
			var val = RoundForDialect(DateTime.UtcNow);
			var expected = RoundForDialect(DateTime.SpecifyKind(val, DateTimeKind.Utc));

			var basic = new DateTimeClass
			{
				Id = 1,
				UtcDateTimeValue = val
			};

			using (var s = OpenSession())
			{
				s.Save(basic);
				s.Flush();
			}

			using (var s = OpenSession())
			{
				basic = (DateTimeClass) s.Load(typeof(DateTimeClass), 1);

				Assert.AreEqual(DateTimeKind.Utc, basic.UtcDateTimeValue.Value.Kind);
				Assert.AreEqual(expected, basic.UtcDateTimeValue);

				s.Delete(basic);
				s.Flush();
			}
		}
	}
}

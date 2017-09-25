using System;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// The Unit Tests for the UtcDateTimeType.
	/// </summary>
	[TestFixture]
	public class LocalDateTimeTypeFixture : TypeFixtureBase
	{
		protected override string TypeName => "DateTime";

		[Test]
		public void ReadWrite()
		{
			var val = RoundForDialect(DateTime.UtcNow);
			var expected = RoundForDialect(DateTime.SpecifyKind(val, DateTimeKind.Local));
			var basic = new DateTimeClass
			{
				Id = 1,
				LocalDateTimeValue = val
			};

			using (var s = OpenSession())
			{
				s.Save(basic);
				s.Flush();
			}

			using (var s = OpenSession())
			{
				basic = s.Load<DateTimeClass>(1);

				Assert.AreEqual(DateTimeKind.Local, basic.LocalDateTimeValue.Value.Kind);
				Assert.AreEqual(expected, basic.LocalDateTimeValue);

				s.Delete(basic);
				s.Flush();
			}
		}
	}
}

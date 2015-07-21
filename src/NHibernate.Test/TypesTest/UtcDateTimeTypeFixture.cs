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
		protected override string TypeName
		{
			get { return "DateTime"; }
		}

		[Test]
		public void ReadWrite()
		{
			DateTime val = DateTime.UtcNow;
			DateTime expected = new DateTime(val.Year, val.Month, val.Day, val.Hour, val.Minute, val.Second, DateTimeKind.Utc);

			DateTimeClass basic = new DateTimeClass();
			basic.Id = 1;
			basic.UtcDateTimeValue = val;

			ISession s = OpenSession();
			s.Save(basic);
			s.Flush();
			s.Close();

			s = OpenSession();
			basic = (DateTimeClass) s.Load(typeof (DateTimeClass), 1);

			Assert.AreEqual(DateTimeKind.Utc, basic.UtcDateTimeValue.Value.Kind);
			Assert.AreEqual(expected, basic.UtcDateTimeValue.Value);

			s.Delete(basic);
			s.Flush();
			s.Close();
		}
	}
}

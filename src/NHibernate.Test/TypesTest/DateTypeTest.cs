using System.Collections.Generic;
using NHibernate.Dialect;
using NHibernate.Type;
using NUnit.Framework;
using System;

namespace NHibernate.Test.TypesTest
{
	[TestFixture]
	public class DateTypeTest
	{
		[Test, Obsolete("Testing obsolete SetParameterValues")]
		public void WhenNoParameterThenDefaultValueIsBaseDateValue()
		{
			var dateType = new DateType();
			Assert.That(dateType.DefaultValue, Is.EqualTo(DateType.BaseDateValue));
		}

		[Test, Obsolete("Testing obsolete SetParameterValues")]
		public void WhenSetParameterThenDefaultValueIsParameterValue()
		{
			var dateType = new DateType();
			dateType.SetParameterValues(new Dictionary<string, string> { { DateType.BaseValueParameterName, "0001/01/01" } });
			Assert.That(dateType.DefaultValue, Is.EqualTo(DateTime.MinValue));
		}

		[Test, Obsolete("Testing obsolete SetParameterValues")]
		public void WhenSetParameterNullThenNotThrow()
		{
			var dateType = new DateType();
			Assert.That(() => dateType.SetParameterValues(null), Throws.Nothing);
		}
	}

	[TestFixture]
	public class DateTypeFixture : AbstractDateTimeTypeFixture
	{
		protected override string TypeName => "Date";
		protected override AbstractDateTimeType Type => NHibernateUtil.Date;
		protected override bool RevisionCheck => false;
		protected override long DateAccuracyInTicks => TimeSpan.TicksPerDay;

		protected override DateTime GetTestDate(DateTimeKind kind)
			=> base.GetTestDate(kind).Date;

		protected override DateTime GetSameDate(DateTime original)
		{
			var date = base.GetSameDate(original);
			return date.AddHours(date.Hour < 12 ? 12 : -12);
		}

		[Test]
		public void ShouldBeDateType()
		{
			if (!(Dialect is MsSql2008Dialect))
			{
				Assert.Ignore("This test does not apply to " + Dialect);
			}
			var sqlType = Dialect.GetTypeName(Type.SqlType);
			Assert.That(sqlType.ToLowerInvariant(), Is.EqualTo("date"));
		}

		[Test]
		public void ReadWriteNormal()
		{
			var expected = DateTime.Today;

			ReadWrite(expected);
		}

		[Test]
		public void ReadWriteMin()
		{
			var expected = Sfi.ConnectionProvider.Driver.MinDate;

			ReadWrite(expected);
		}

		[Test]
		public void ReadWriteYear750()
		{
			var expected = new DateTime(750, 5, 13);
			if (Sfi.ConnectionProvider.Driver.MinDate > expected)
			{
				Assert.Ignore($"The driver does not support dates below {Sfi.ConnectionProvider.Driver.MinDate:O}");
			}
			ReadWrite(expected);
		}

		private void ReadWrite(DateTime expected)
		{
			// Add an hour to check it is correctly ignored once read back from db.
			var basic = new DateTimeClass { Id = AdditionalDateId, Value = expected.AddHours(1) };
			using (var s = OpenSession())
			{
				s.Save(basic); 
				s.Flush();
			}
			using (var s = OpenSession())
			{
				basic = s.Get<DateTimeClass>(AdditionalDateId);
				Assert.That(basic.Value, Is.EqualTo(expected));
				s.Delete(basic);
				s.Flush();
			}
		}

		// They furthermore cause some issues with Sql Server Ce, which switch DbType.Date for DbType.DateTime
		// on its parameters.
		[Ignore("Test relevant for datetime, not for date.")]
		public override void SaveUseExpectedSqlType()
		{
		}

		[Ignore("Test relevant for datetime, not for date.")]
		public override void UpdateUseExpectedSqlType()
		{
		}

		[Ignore("Test relevant for datetime, not for date.")]
		public override void QueryUseExpectedSqlType()
		{
		}
	}
}

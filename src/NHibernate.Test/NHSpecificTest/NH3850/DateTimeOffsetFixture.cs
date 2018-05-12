using System.Collections;
using System.Data;
using System.Linq;
using NHibernate.Driver;
using NHibernate.Linq;
using NHibernate.SqlTypes;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3850
{
	[TestFixture]
	public class DateTimeOffsetFixture : FixtureBase
	{
		protected override IList Mappings => new[] { $"NHSpecificTest.{BugNumber}.DateTimeOffsetMappings.hbm.xml" };

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return TestDialect.SupportsSqlType(new SqlType(DbType.DateTimeOffset));
		}

		protected override bool AppliesTo(Engine.ISessionFactoryImplementor factory)
		{
			// Cannot handle DbType.DateTimeOffset via ODBC.
			return !(factory.ConnectionProvider.Driver is OdbcDriver);
		}

		protected override void Max<DC>(int? expectedResult)
		{
			using (var session = OpenSession())
			{
				var dcQuery = session.Query<DC>();
				var dateWithOffset = dcQuery.Max(dc => dc.DateTimeOffset);
				var futureDateWithOffset = dcQuery.ToFutureValue(qdc => qdc.Max(dc => dc.DateTimeOffset));
				if (expectedResult.HasValue)
				{
					Assert.That(dateWithOffset, Is.GreaterThan(TestDateWithOffset), "DateTimeOffset max has failed");
					Assert.That(
						futureDateWithOffset.Value,
						Is.GreaterThan(TestDateWithOffset),
						"Future DateTimeOffset max has failed");
				}
				else
				{
					Assert.That(dateWithOffset, Is.Null, "DateTimeOffset max has failed");
					Assert.That(futureDateWithOffset.Value, Is.Null, "Future DateTimeOffset max has failed");
				}
			}
		}

		protected override void Min<DC>(int? expectedResult)
		{
			using (var session = OpenSession())
			{
				var dcQuery = session.Query<DC>();
				var dateWithOffset = dcQuery.Min(dc => dc.DateTimeOffset);
				var futureDateWithOffset = dcQuery.ToFutureValue(qdc => qdc.Min(dc => dc.DateTimeOffset));
				if (expectedResult.HasValue)
				{
					Assert.That(dateWithOffset, Is.LessThan(TestDateWithOffset), "DateTimeOffset min has failed");
					Assert.That(futureDateWithOffset.Value, Is.LessThan(TestDateWithOffset), "Future DateTimeOffset min has failed");
				}
				else
				{
					Assert.That(dateWithOffset, Is.Null, "DateTimeOffset min has failed");
					Assert.That(futureDateWithOffset.Value, Is.Null, "Future DateTimeOffset min has failed");
				}
			}
		}
	}
}

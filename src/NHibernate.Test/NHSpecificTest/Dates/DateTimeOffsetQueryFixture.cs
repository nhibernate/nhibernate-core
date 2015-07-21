using System;
using System.Collections;
using System.Data;
using System.Linq;
using NHibernate.Driver;
using NHibernate.Type;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;
using NHibernate.Linq;

namespace NHibernate.Test.NHSpecificTest.Dates
{
	[TestFixture]
	public class DateTimeOffsetQueryFixture : FixtureBase
	{
		protected override IList Mappings
		{
			get { return new[] { "NHSpecificTest.Dates.Mappings.DateTimeOffset.hbm.xml" }; }
		}

		protected override bool AppliesTo(Engine.ISessionFactoryImplementor factory)
		{
			// Cannot handle DbType.DateTimeOffset via ODBC.
			if (factory.ConnectionProvider.Driver is OdbcDriver)
				return false;

			return base.AppliesTo(factory);
		}

		protected override DbType? AppliesTo()
		{
			return DbType.DateTimeOffset;
		}

		protected override void Configure(Cfg.Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SetProperty(Environment.ShowSql, "true");
		}


		protected override void OnSetUp()
		{
			base.OnSetUp();

			var dates1 = new AllDates { Sql_datetimeoffset = new DateTimeOffset(2012, 11, 1, 1, 0, 0, TimeSpan.FromHours(1)) };
			var dates2 = new AllDates { Sql_datetimeoffset = new DateTimeOffset(2012, 11, 1, 2, 0, 0, TimeSpan.FromHours(3)) };

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Save(dates1);
				s.Save(dates2);
				tx.Commit();
			}
		}


		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from AllDates");
				tx.Commit();
			}
		}


		[Test]
		public void CanQueryWithCastInHql()
		{
			using (ISession s = OpenSession())
			using (s.BeginTransaction())
			{
				var datesRecovered = s.CreateQuery("select cast(min(Sql_datetimeoffset), datetimeoffset) from AllDates").UniqueResult<DateTimeOffset>();
				Assert.That(datesRecovered, Is.EqualTo(new DateTimeOffset(2012, 11, 1, 2, 0, 0, TimeSpan.FromHours(3))));
			}
		}


		[Test(Description = "NH-3357")]
		public void CanQueryWithAggregateInLinq()
		{
			using (ISession s = OpenSession())
			using (s.BeginTransaction())
			{
				// The Min() will generate a HqlCast, which requires that the linq
				// provider can find a HQL name for datetimeoffset.

				var datesRecovered = (from allDates in s.Query<AllDates>()
									  select allDates.Sql_datetimeoffset).Min();

				Assert.That(datesRecovered, Is.EqualTo(new DateTimeOffset(2012, 11, 1, 2, 0, 0, TimeSpan.FromHours(3))));
			}

		}
	}
}

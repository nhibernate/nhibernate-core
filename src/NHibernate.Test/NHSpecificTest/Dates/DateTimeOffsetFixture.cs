using System;
using System.Collections;
using System.Data;
using NHibernate.Driver;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.Dates
{
	[TestFixture]
	public class DateTimeOffsetFixture : FixtureBase
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

		[Test]
		public void SavingAndRetrievingTest()
		{
			DateTimeOffset NowOS = DateTimeOffset.Now;

			AllDates dates = new AllDates { Sql_datetimeoffset = NowOS };

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Save(dates);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				var datesRecovered = s.CreateQuery("from AllDates").UniqueResult<AllDates>();
				Assert.That(datesRecovered.Sql_datetimeoffset, Is.EqualTo(NowOS));
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				var datesRecovered = s.CreateQuery("from AllDates").UniqueResult<AllDates>();
				s.Delete(datesRecovered);
				tx.Commit();
			}
		}

		[Test]
		public void WhenEqualTicksThenShouldMatchIsEqual()
		{
			var type = new DateTimeOffsetType();
			var now = DateTimeOffset.Now;
			Assert.That(type.IsEqual(new DateTimeOffset(now.Ticks, now.Offset), new DateTimeOffset(now.Ticks, now.Offset)), Is.True);
		}

		[Test]
		public void WhenNotEqualTicksThenShouldNotMatchIsEqual()
		{
			var type = new DateTimeOffsetType();
			var now = DateTimeOffset.Now;
			Assert.That(type.IsEqual(new DateTimeOffset(now.Ticks - 1, now.Offset), new DateTimeOffset(now.Ticks, now.Offset)), Is.False);
		}

		[Test]
		public void HashCodeShouldHaveSameBehaviorOfNetType()
		{
			var type = new DateTimeOffsetType();
			var now = DateTimeOffset.Now;
			var exactClone = new DateTimeOffset(now.Ticks, now.Offset);
			Assert.That((now.GetHashCode() == exactClone.GetHashCode()), Is.EqualTo(now.GetHashCode() == type.GetHashCode(exactClone, EntityMode.Poco)));
		}

		[Test]
		public void Next()
		{
			var type = NHibernateUtil.DateTimeOffset;
			var current = DateTimeOffset.Now.AddTicks(-1);
			object next = type.Next(current, null);

			Assert.That(next, Is.TypeOf<DateTimeOffset>().And.Property("Ticks").GreaterThan(current.Ticks));
		}

		[Test]
		public void Seed()
		{
			var type = NHibernateUtil.DateTimeOffset;
			Assert.That(type.Seed(null), Is.TypeOf<DateTimeOffset>());
		}

	}
}
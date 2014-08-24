using System;
using System.Collections;
using System.Data;
using NHibernate.Driver;
using NHibernate.Type;
using NUnit.Framework;
using SharpTestsEx;

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
				datesRecovered.Sql_datetimeoffset.Should().Be(NowOS);
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
			type.IsEqual(new DateTimeOffset(now.Ticks, now.Offset), new DateTimeOffset(now.Ticks, now.Offset)).Should().Be.True();
		}

		[Test]
		public void WhenNotEqualTicksThenShouldNotMatchIsEqual()
		{
			var type = new DateTimeOffsetType();
			var now = DateTimeOffset.Now;
			type.IsEqual(new DateTimeOffset(now.Ticks - 1, now.Offset), new DateTimeOffset(now.Ticks, now.Offset)).Should().Be.False();
		}

		[Test]
		public void HashCodeShouldHaveSameBehaviorOfNetType()
		{
			var type = new DateTimeOffsetType();
			var now = DateTimeOffset.Now;
			var exactClone = new DateTimeOffset(now.Ticks, now.Offset);
			(now.GetHashCode() == exactClone.GetHashCode()).Should().Be.EqualTo(now.GetHashCode() == type.GetHashCode(exactClone, EntityMode.Poco));
		}

		[Test]
		public void Next()
		{
			var type = (DateTimeOffsetType)NHibernateUtil.DateTimeOffset;
			var current = DateTimeOffset.Now.AddTicks(-1);
			object next = type.Next(current, null);

			next.Should().Be.OfType<DateTimeOffset>().And.ValueOf.Ticks.Should().Be.GreaterThan(current.Ticks);
		}

		[Test]
		public void Seed()
		{
			var type = (DateTimeOffsetType)NHibernateUtil.DateTimeOffset;
			type.Seed(null).Should().Be.OfType<DateTimeOffset>();
		}

	}
}
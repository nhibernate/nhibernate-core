using System;
using System.Collections;
using System.Data;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.Dates
{
	[TestFixture]
	public class DateTimeOffSetFixture : FixtureBase
	{
		protected override IList Mappings
		{
			get { return new[] {"NHSpecificTest.Dates.Mappings.DateTimeOffset.hbm.xml"}; }
		}

		protected override DbType? AppliesTo()
		{
			return DbType.DateTimeOffset;
		}

		[Test]
		public void SavingAndRetrievingTest()
		{
			DateTimeOffset NowOS = DateTimeOffset.Now;

			SavingAndRetrievingAction(new AllDates {Sql_datetimeoffset = NowOS},
			                          entity => DateTimeAssert.AreEqual(entity.Sql_datetimeoffset, NowOS));
		}
	}
}
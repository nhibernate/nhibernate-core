using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.Dates
{
	[TestFixture]
	public class DateTimeOffSetFixture : DatesFixtureBase
	{
		protected override IList Mappings
		{
			get { return new[] {"NHSpecificTest.Dates.Mappings.DateTimeOffset.hbm.xml"}; }
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
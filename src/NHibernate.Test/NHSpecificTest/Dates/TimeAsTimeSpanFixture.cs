using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.Dates
{
	[TestFixture]
	public class TimeAsTimeSpanFixture : FixtureBase
	{
		protected override IList Mappings
		{
			get { return new[] {"NHSpecificTest.Dates.Mappings.TimeAsTimeSpan.hbm.xml"}; }
		}

		[Test]
		public void SavingAndRetrievingTest()
		{
			TimeSpan now = DateTime.Parse("23:59:59").TimeOfDay;

			SavingAndRetrievingAction(new AllDates { Sql_TimeAsTimeSpan = now },
			                          entity =>
			                          	{
											Assert.AreEqual(entity.Sql_TimeAsTimeSpan.Hours, now.Hours);
											Assert.AreEqual(entity.Sql_TimeAsTimeSpan.Minutes, now.Minutes);
											Assert.AreEqual(entity.Sql_TimeAsTimeSpan.Seconds, now.Seconds);
			                          	});
		}
	}
}
using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.Dates
{
	[TestFixture]
	public class TimeFixture : FixtureBase
	{
		protected override IList Mappings
		{
			get { return new[] {"NHSpecificTest.Dates.Mappings.Time.hbm.xml"}; }
		}

		[Test]
		public void SavingAndRetrievingTest()
		{
			DateTime now = DateTime.Parse("23:59:59");

			SavingAndRetrievingAction(new AllDates {Sql_time = now},
			                          entity =>
			                          	{
			                          		Assert.AreEqual(entity.Sql_time.Hour, now.Hour);
			                          		Assert.AreEqual(entity.Sql_time.Minute, now.Minute);
			                          		Assert.AreEqual(entity.Sql_time.Second, now.Second);
			                          	});
		}
	}
}
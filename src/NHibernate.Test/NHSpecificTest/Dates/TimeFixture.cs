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

		[Test,Ignore]
		public void SavingAndRetrievingTest()
		{
			TimeSpan Now = TimeSpan.MaxValue;
			SavingAndRetrievingAction(new AllDates {Sql_time = Now},
			                          entity => Assert.AreEqual(entity.Sql_time, Now));
		}
	}
}
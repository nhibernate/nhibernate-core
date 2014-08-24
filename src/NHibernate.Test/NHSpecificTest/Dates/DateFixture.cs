using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.Dates
{
	[TestFixture]
	public class DateFixture : FixtureBase
	{
		protected override IList Mappings
		{
			get { return new[] {"NHSpecificTest.Dates.Mappings.Date.hbm.xml"}; }
		}

		[Test]
		public void SavingAndRetrievingTest()
		{
			DateTime Now = DateTime.Now;
			SavingAndRetrievingAction(new AllDates {Sql_date = Now},
			                          entity => DateTimeAssert.AreEqual(entity.Sql_date, Now, true));
		}
	}
}
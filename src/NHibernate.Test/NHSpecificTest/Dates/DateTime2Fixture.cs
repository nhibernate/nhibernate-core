using System;
using System.Collections;
using System.Data;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.Dates
{
	[TestFixture]
	public class DateTime2Fixture : FixtureBase
	{
		protected override IList Mappings
		{
			get { return new[] {"NHSpecificTest.Dates.Mappings.DateTime2.hbm.xml"}; }
		}

		protected override DbType? AppliesTo()
		{
			return DbType.DateTime2;
		}

		[Test]
		public void SavingAndRetrievingTest()
		{
			DateTime Now = DateTime.Now;

			SavingAndRetrievingAction(new AllDates {Sql_datetime2 = Now},
			                          entity => DateTimeAssert.AreEqual(entity.Sql_datetime2, Now));


			SavingAndRetrievingAction(new AllDates { Sql_datetime2 = DateTime.MinValue },
									  entity => DateTimeAssert.AreEqual(entity.Sql_datetime2, DateTime.MinValue));

			SavingAndRetrievingAction(new AllDates { Sql_datetime2 = DateTime.MaxValue },
									  entity => DateTimeAssert.AreEqual(entity.Sql_datetime2, DateTime.MaxValue));
		}

		[Test]
		public void SaveMillisecond()
		{
			DateTime datetime2 = DateTime.MinValue.AddMilliseconds(123);

			SavingAndRetrievingAction(new AllDates { Sql_datetime2 = datetime2 },
																entity => Assert.That(entity.Sql_datetime2, Is.EqualTo(datetime2)));
		}
	}
}
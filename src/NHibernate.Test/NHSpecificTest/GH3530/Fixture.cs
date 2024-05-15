using System;
using System.Globalization;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3530
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private CultureInfo initialCulture;

		[OneTimeSetUp]
		public void FixtureSetup()
		{
			initialCulture = CurrentCulture;
		}

		[OneTimeTearDown]
		public void FixtureTearDown()
		{
			CurrentCulture = initialCulture;
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[TestCaseSource(nameof(GetTestCases))]
		public void TestLocales(CultureInfo from, CultureInfo to)
		{
			DateTime leapDay = new DateTime(2024, 2, 29, new GregorianCalendar(GregorianCalendarTypes.USEnglish));
			double doubleValue = 12.3f;
			int intValue = 4;
			object id;

			CurrentCulture = from;
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var entity = new LocaleEntity()
				{
					DateTimeValue = leapDay,
					DoubleValue = doubleValue,
					IntegerValue = intValue,
				};

				id = session.Save(entity);
				tx.Commit();
			}

			CurrentCulture = to;
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var entity = session.Get<LocaleEntity>(id);

				Assert.AreEqual(leapDay, entity.DateTimeValue);
				Assert.AreEqual(intValue, entity.IntegerValue);
				Assert.True(doubleValue - entity.DoubleValue < double.Epsilon);
			}
		}

		private CultureInfo CurrentCulture
		{
			get
			{
				return CultureInfo.CurrentCulture;
			}
			set
			{
				CultureInfo.CurrentCulture = value;
			}
		}

		public static object[][] GetTestCases()
		{
			return [
				[new CultureInfo("en-US"), new CultureInfo("de-DE")],
				[new CultureInfo("en-US"), new CultureInfo("ar-SA", false)],
				[new CultureInfo("en-US"), new CultureInfo("th-TH", false)],
			];
		}
	}
}

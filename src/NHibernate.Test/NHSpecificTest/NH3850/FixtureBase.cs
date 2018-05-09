using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3850
{
	[TestFixture]
	public abstract class FixtureBase : BugTestCase
	{
		protected const string SearchName1 = "name";
		protected const string SearchName2 = "name2";
		protected const int TotalEntityCount = 10;
		protected readonly DateTime TestDate = DateTime.Now;
		protected readonly DateTimeOffset TestDateWithOffset = DateTimeOffset.Now;

		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (var session = OpenSession())
			{
				var dateTime1 = TestDate.AddDays(-1);
				var dateTime2 = TestDate.AddDays(1);
				var dateTimeOffset1 = TestDateWithOffset.AddDays(-1);
				var dateTimeOffset2 = TestDateWithOffset.AddDays(1);
				Action<DomainClassBase> init1 = dc =>
				{
					dc.Id = 1;
					dc.Name = SearchName1;
					dc.Integer = 1;
					dc.Long = 1;
					dc.Decimal = 1;
					dc.Double = 1;
					dc.DateTime = dateTime1;
					dc.DateTimeOffset = dateTimeOffset1;
					dc.NonNullableDecimal = 1;
				};
				Action<DomainClassBase> init2 = dc =>
				{
					dc.Id = 2;
					dc.Name = SearchName2;
					dc.Integer = 2;
					dc.Long = 2;
					dc.Decimal = 2;
					dc.Double = 2;
					dc.DateTime = dateTime2;
					dc.DateTimeOffset = dateTimeOffset2;
					dc.NonNullableDecimal = 2;
				};

				DomainClassBase entity = new DomainClassBExtendedByA();
				init1(entity);
				session.Save(entity);
				entity = new DomainClassBExtendedByA();
				init2(entity);
				session.Save(entity);

				entity = new DomainClassCExtendedByD();
				init1(entity);
				session.Save(entity);
				entity = new DomainClassCExtendedByD();
				init2(entity);
				session.Save(entity);

				entity = new DomainClassE();
				init1(entity);
				session.Save(entity);
				entity = new DomainClassE();
				init2(entity);
				session.Save(entity);

				entity = new DomainClassGExtendedByH();
				init1(entity);
				session.Save(entity);
				entity = new DomainClassGExtendedByH();
				init2(entity);
				session.Save(entity);
				entity = new DomainClassHExtendingG
				{
					Id = 3,
					Name = SearchName1,
					Integer = 3,
					Long = 3,
					Decimal = 3,
					Double = 3,
					DateTime = dateTime1,
					DateTimeOffset = dateTimeOffset1,
					NonNullableDecimal = 3
				};
				session.Save(entity);
				entity = new DomainClassHExtendingG
				{
					Id = 4,
					Name = SearchName2,
					Integer = 4,
					Long = 4,
					Decimal = 4,
					Double = 4,
					DateTime = dateTime2,
					DateTimeOffset = dateTimeOffset2,
					NonNullableDecimal = 4
				};
				session.Save(entity);

				session.Flush();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (var session = OpenSession())
			{
				var hql = "from System.Object";
				session.Delete(hql);
				session.Flush();
			}
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void MaxBBase()
		{
			Max<DomainClassBExtendedByA>(2);
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void MaxCBase()
		{
			Max<DomainClassCExtendedByD>(2);
		}

		// Non-reg case
		[Test]
		public void MaxE()
		{
			Max<DomainClassE>(2);
		}

		// Non-reg case
		[Test]
		public void MaxF()
		{
			Max<DomainClassF>(null);
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void MaxGBase()
		{
			Max<DomainClassGExtendedByH>(4);
		}

		protected abstract void Max<TDc>(int? expectedResult) where TDc : DomainClassBase;

		// Failing case till NH-3850 is fixed
		[Test]
		public void MinBBase()
		{
			Min<DomainClassBExtendedByA>(1);
		}

		// Failing case till NH-3850 is fixed
		[Test]
		public void MinCBase()
		{
			Min<DomainClassCExtendedByD>(1);
		}

		// Non-reg case
		[Test]
		public void MinE()
		{
			Min<DomainClassE>(1);
		}

		// Non-reg case
		[Test]
		public void MinF()
		{
			Min<DomainClassF>(null);
		}

		// Non-reg case
		[Test]
		public void MinGBase()
		{
			Min<DomainClassGExtendedByH>(1);
		}

		protected abstract void Min<TDc>(int? expectedResult) where TDc : DomainClassBase;
	}
}

using System;
using System.Globalization;
using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3961
{
	[TestFixture]
	public class DateParametersComparedTo : BugTestCase
	{
		private DateTime _testDate;
		private CultureInfo _backupCulture;
		private CultureInfo _backupUICulture;
		private readonly CultureInfo _testCulture = CultureInfo.GetCultureInfo("fr-FR");

		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				// day > 12 for ensuring a mdy/dmy mix-up would cause a failure.
				_testDate = new DateTime(2017, 03, 15);
				var e1 = new Entity { Name = "Bob", NullableDateTime = _testDate, NonNullableDateTime = _testDate };
				session.Save(e1);

				var e2 = new Entity { Name = "Sally", NullableDateTime = _testDate.AddDays(1), NonNullableDateTime = _testDate.AddDays(1) };
				session.Save(e2);

				session.Flush();
				transaction.Commit();
			}

			_backupCulture = CultureInfo.CurrentCulture;
			_backupUICulture = CultureInfo.CurrentUICulture;
			// "CultureInfo.CurrentCulture =": Fx 4.6 only; affect trough Thread.CurrentThread instead if in need of supporting a previous Fx.
			// This test needs a culture using a dmy date format. If the test system does not support fr-FR, try find another one...
			// This test assumes the SQL user language is set as English, otherwise it may not showcase the failure.
			CultureInfo.CurrentCulture = _testCulture;
			CultureInfo.CurrentUICulture = _testCulture;
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
			CultureInfo.CurrentCulture = _backupCulture;
			CultureInfo.CurrentUICulture = _backupUICulture;
		}

		// Non-reg test case
		[Test]
		public void NonNullableMappedAsDateShouldBeCultureAgnostic()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Where(e => e.NonNullableDateTime == _testDate.MappedAs(NHibernateUtil.Date))
					.ToList();

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual("Bob", result[0].Name);
			}
		}

		// Non-reg test case
		[Test]
		public void NonNullableMappedAsDateShouldIgnoreTime()
		{
			using (ISession session = OpenSession())
			{
				var result = session.Query<Entity>()
					.Where(e => e.NonNullableDateTime == _testDate.AddMinutes(10).MappedAs(NHibernateUtil.Date))
					.ToList();

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual("Bob", result[0].Name);
			}
		}

		// Non-reg test case
		[Test]
		public void NonNullableMappedAsDateTimeShouldBeCultureAgnostic()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Where(e => e.NonNullableDateTime == _testDate.MappedAs(NHibernateUtil.DateTime))
					.ToList();

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual("Bob", result[0].Name);
			}
		}

		// Non-reg test case
		[Test]
		[Obsolete]
		public void NonNullableMappedAsTimestampShouldBeCultureAgnostic()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Where(e => e.NonNullableDateTime == _testDate.MappedAs(NHibernateUtil.Timestamp))
					.ToList();

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual("Bob", result[0].Name);
			}
		}

		// Non-reg test case
		[Test]
		public void NonNullableParameterValueShouldNotBeCachedWithMappedAsAnd()
		{
			// Dodges the query parameter formatting bug for showcasing the parameter value bug
			CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
			CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;
			try
			{
				using (ISession session = OpenSession())
				{
					var result = session.Query<Entity>()
						.Where(e => e.NonNullableDateTime == _testDate.MappedAs(NHibernateUtil.DateTime))
						.ToList();

					Assert.AreEqual(1, result.Count);
					Assert.AreEqual("Bob", result[0].Name);

					var testDate = _testDate.AddMinutes(10);
					result = session.Query<Entity>()
						.Where(e => e.NonNullableDateTime == testDate.MappedAs(NHibernateUtil.DateTime))
						.ToList();

					CollectionAssert.IsEmpty(result);
				}
			}
			finally
			{
				CultureInfo.CurrentCulture = _testCulture;
				CultureInfo.CurrentUICulture = _testCulture;
			}
		}

		// Non-reg test case
		[Test]
		public void NonNullableShouldBeCultureAgnostic()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Where(e => e.NonNullableDateTime == _testDate)
					.ToList();

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual("Bob", result[0].Name);
			}
		}

		// Failing test case till NH-3961 is fixed
		[Test]
		public void NullableMappedAsDateShouldBeCultureAgnostic()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Where(e => e.NullableDateTime == _testDate.MappedAs(NHibernateUtil.Date))
					.ToList();

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual("Bob", result[0].Name);
			}
		}

		// Failing test case till NH-3961 is fixed
		[Test]
		public void NullableMappedAsDateShouldIgnoreTime()
		{
			var testDate = _testDate.AddMinutes(10);
			using (ISession session = OpenSession())
			{
				var result = session.Query<Entity>()
					.Where(e => e.NullableDateTime == testDate.MappedAs(NHibernateUtil.Date))
					.ToList();

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual("Bob", result[0].Name);
			}
		}

		// Failing test case till NH-3961 is fixed
		[Test]
		public void NullableMappedAsDateTimeShouldBeCultureAgnostic()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Where(e => e.NullableDateTime == _testDate.MappedAs(NHibernateUtil.DateTime))
					.ToList();

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual("Bob", result[0].Name);
			}
		}

		// Failing test case till NH-3961 is fixed
		[Test]
		[Obsolete]
		public void NullableMappedAsTimestampShouldBeCultureAgnostic()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Where(e => e.NullableDateTime == _testDate.MappedAs(NHibernateUtil.Timestamp))
					.ToList();

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual("Bob", result[0].Name);
			}
		}

		// Failing test case till NH-3961 is fixed
		[Test]
		public void NullableParameterValueShouldNotBeCachedWithMappedAs()
		{
			// Dodges the query parameter formatting bug for showcasing the parameter value bug
			CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
			CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;
			try
			{
				using (ISession session = OpenSession())
				{
					var result = session.Query<Entity>()
						.Where(e => e.NullableDateTime == _testDate.MappedAs(NHibernateUtil.DateTime))
						.ToList();

					Assert.AreEqual(1, result.Count);
					Assert.AreEqual("Bob", result[0].Name);

					var testDate = _testDate.AddMinutes(10);
					result = session.Query<Entity>()
						.Where(e => e.NullableDateTime == testDate.MappedAs(NHibernateUtil.DateTime))
						.ToList();

					CollectionAssert.IsEmpty(result);
				}
			}
			finally
			{
				CultureInfo.CurrentCulture = _testCulture;
				CultureInfo.CurrentUICulture = _testCulture;
			}
		}

		// Non-reg test case
		[Test]
		public void NullableShouldBeCultureAgnostic()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Where(e => e.NullableDateTime == _testDate)
					.ToList();

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual("Bob", result[0].Name);
			}
		}
	}
}

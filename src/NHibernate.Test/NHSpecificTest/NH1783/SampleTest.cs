using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1783
{
	[TestFixture]
	public class SampleTest : BugTestCase
	{
		[Test]
		public void DatePropertyShouldBeStoredWithoutTimePart()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				var entity = new DomainClass {Id = 1, BirthDate = new DateTime(1950, 2, 13, 3, 12, 10)};
				session.Save(entity);
				tx.Commit();
			}

			using (ISession session = OpenSession())
			{
				// upload the result using DateTime type to verify it does not have the time-part.
				var l = session.CreateSQLQuery("SELECT BirthDate AS bd FROM DomainClass")
					.AddScalar("bd",NHibernateUtil.DateTime).List();
				var actual = (DateTime) l[0];
				var expected = new DateTime(1950, 2, 13);
				Assert.That(actual, Is.EqualTo(expected));
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.CreateQuery("delete from DomainClass").ExecuteUpdate();
				tx.Commit();
			}
		}
	}
}
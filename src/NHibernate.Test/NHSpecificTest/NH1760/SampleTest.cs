using System.Collections.Generic;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1760
{
	[TestFixture]
	public class SampleTest : BugTestCase
	{
		private void FillDb()
		{
			using (ISession session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var customer = new Customer {Name = "Alkampfer"};
				session.Save(customer);
				var testClass = new TestClass { Id = new TestClassId { Customer = customer, SomeInt = 42 }, Value = "TESTVALUE" };
				session.Save(testClass);
				tx.Commit();
			}
		}

		private void Cleanup()
		{
			using (ISession session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.CreateQuery("delete from TestClass").ExecuteUpdate();
				session.CreateQuery("delete from Customer").ExecuteUpdate();
				tx.Commit();
			}
		}

		[Test]
		public void CanUseCriteria()
		{
			FillDb();
			int hqlCount;
			int criteriaCount;
			using (ISession session = OpenSession())
			{
				IList<TestClass> retvalue =
					session.CreateQuery("Select tc from TestClass tc join tc.Id.Customer cu where cu.Name = :name").SetString("name", "Alkampfer")
						.List<TestClass>();
				hqlCount = retvalue.Count;
			}

			using (ISession session = OpenSession())
			{
				ICriteria c =
					session.CreateCriteria(typeof (TestClass)).CreateAlias("Id.Customer", "IdCust").Add(Restrictions.Eq("IdCust.Name",
					                                                                                                    "Alkampfer"));
				IList<TestClass> retvalue = c.List<TestClass>();
				criteriaCount = retvalue.Count;
			}
			Assert.That(criteriaCount, Is.EqualTo(1));
			Assert.That(criteriaCount, Is.EqualTo(hqlCount));

			Cleanup();
		}

		[Test]
		public void TheJoinShouldBeOptional()
		{
			FillDb();
			int criteriaCount;

			using (ISession session = OpenSession())
			{
				using (var ls = new SqlLogSpy())
				{
					ICriteria c = session.CreateCriteria(typeof(TestClass));
					IList<TestClass> retvalue = c.List<TestClass>();
					Assert.That(ls.GetWholeLog(), Is.Not.StringContaining("join"));
					criteriaCount = retvalue.Count;
				}
			}
			Assert.That(criteriaCount, Is.EqualTo(1));

			Cleanup();
		}
	}
}
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
				var testClass = new TestClass {Id = new TestClassId{Customer = customer, SomeInt = 42}, Value = "TESTVALUE"};
				session.Save(testClass);
				tx.Commit();
			}
		}

		private void Cleanup()
		{
			using (ISession session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.CreateQuery("from TestClass").ExecuteUpdate();
				session.CreateQuery("from Customer").ExecuteUpdate();
				tx.Commit();
			}
		}

		[Test, Ignore("Not fixed yet.")]
		public void CanUseCriteria()
		{
			FillDb();
			int hqlCount;
			int criteriaCount;
			using (ISession session = OpenSession())
			{
				IList<TestClass> retvalue =
					session.CreateQuery("Select tc from TestClass tc where tc.Id.Customer.Name = :name").SetString("name", "Alkampfer")
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
			Assert.That(hqlCount == criteriaCount);
			Assert.That(hqlCount, Is.EqualTo(1));

			Cleanup();
		}
	}
}
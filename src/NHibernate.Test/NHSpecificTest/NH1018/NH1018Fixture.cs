using System;
using System.Collections;

using NUnit.Framework;
using NHibernate.Criterion;

namespace NHibernate.Test.NHSpecificTest.NH1018
{
	[TestFixture]
	public class NH1018Fixture : BugTestCase
	{
		[Test]
		public void Test()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				Employer emr1 = new Employer("Test Employer 1");
				Employer emr2 = new Employer("Test Employer 2");

				Employee[] employees = new Employee[5];

				for (int i = 0; i < employees.Length; i++)
				{
					employees[i] = new Employee("Test Employee " + (i + 1).ToString());
				}

				emr1.AddEmployee(employees[0]);
				emr1.AddEmployee(employees[1]);
				emr1.AddEmployee(employees[2]);
				emr2.AddEmployee(employees[3]);
				emr2.AddEmployee(employees[4]);

				session.Save(emr1);
				session.Save(emr2);

				foreach (Employee emp in employees)
				{
					session.Save(emp);
				}

				tx.Commit();
			}

			using (ISession session = OpenSession())
			{
				IList employers = session.CreateQuery("select emr from Employer emr inner join fetch emr.Employees")
					.SetResultTransformer(CriteriaSpecification.DistinctRootEntity)
					.List();
				Assert.AreEqual(2, employers.Count);
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.Delete("from Employee");
				session.Delete("from Employer");
				tx.Commit();
			}
		}
	}
}
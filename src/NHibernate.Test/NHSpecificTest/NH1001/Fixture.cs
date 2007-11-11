using System;
using System.Collections.Generic;
using System.Text;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1001
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1001"; }
		}

		int employeeId;

		protected override void OnSetUp()
		{
			using (ISession sess = OpenSession())
			using (ITransaction tx = sess.BeginTransaction())
			{
				Department dept = new Department();
				dept.Id = 1;
				dept.Name = "Animal Testing";

				sess.Save(dept);

				Employee emp = new Employee();
				emp.Id = 1;
				emp.FirstName = "John";
				emp.LastName = "Doe";
				emp.Department = dept;

				sess.Save(emp);

				tx.Commit();

				employeeId = emp.Id;
			}

			ExecuteStatement(string.Format("UPDATE EMPLOYEES SET DEPARTMENT_ID = 99999 WHERE EMPLOYEE_ID = {0}", employeeId));
		}

		protected override void OnTearDown()
		{
			using (ISession sess = OpenSession())
			using (ITransaction tx = sess.BeginTransaction())
			{
				sess.Delete("from Employee");
				sess.Delete("from Department");
				tx.Commit();
			}
		}

		[Test]
		[Ignore("To be fixed")]
		public void Test()
		{
			using (ISession sess = OpenSession())
			using (ITransaction tx = sess.BeginTransaction())
			{
				using (SqlLogSpy spy = new SqlLogSpy())
				{
					Employee emp1 = sess.Get<Employee>(employeeId);

					Assert.AreEqual(1, spy.Appender.GetEvents().Length,
					                "Only one SQL statement should have been issued.");
				}

				tx.Commit();
			}
		}
	}
}

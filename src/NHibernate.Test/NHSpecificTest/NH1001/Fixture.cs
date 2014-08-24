using NHibernate.Cfg;
using NUnit.Framework;
using NHibernate.Stat;

namespace NHibernate.Test.NHSpecificTest.NH1001
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1001"; }
		}

		protected override void Configure(Configuration configuration)
		{
			cfg.SetProperty(Environment.GenerateStatistics, "true");
		}


		[Test]
		[Ignore("To be fixed")]
		public void Test()
		{
			int employeeId;
			using (ISession sess = OpenSession())
			using (ITransaction tx = sess.BeginTransaction())
			{
				Department dept = new Department();
				dept.Id = 11;
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

			IStatistics stat = sessions.Statistics;
			stat.Clear();
			using (ISession sess = OpenSession())
			using (ITransaction tx = sess.BeginTransaction())
			{
				sess.Get<Employee>(employeeId);

				Assert.AreEqual(1, stat.PrepareStatementCount);
				tx.Commit();
			}

			using (ISession sess = OpenSession())
			using (ITransaction tx = sess.BeginTransaction())
			{
				sess.Delete("from Employee");
				sess.Delete("from Department");
				tx.Commit();
			}
		}
	}
}

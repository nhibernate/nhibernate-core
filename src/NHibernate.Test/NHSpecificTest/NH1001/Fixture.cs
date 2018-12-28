using NHibernate.Cfg;
using NUnit.Framework;
using NHibernate.Stat;

namespace NHibernate.Test.NHSpecificTest.NH1001
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void Configure(Configuration configuration)
		{
			cfg.SetProperty(Environment.GenerateStatistics, "true");
		}

		private int employeeId;

		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var dept = new Department
				{
					Id = 11,
					Name = "Animal Testing"
				};

				session.Save(dept);

				var emp = new Employee
				{
					Id = 1,
					FirstName = "John",
					LastName = "Doe",
					Department = dept
				};

				session.Save(emp);

				transaction.Commit();

				employeeId = emp.Id;
			}
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
		public void Test()
		{
			ExecuteStatement($"UPDATE EMPLOYEES SET DEPARTMENT_ID = 99999 WHERE EMPLOYEE_ID = {employeeId}");

			IStatistics statistics = Sfi.Statistics;
			statistics.Clear();

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var employee = session.Get<Employee>(employeeId);
				Assert.That(employee.Department, Is.Not.Null);
				Assert.That(statistics.PrepareStatementCount, Is.EqualTo(1));
				transaction.Commit();
			}
		}
	}
}

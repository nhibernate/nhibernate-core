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

				var dept2 = new Department
				{
					Id = 12,
					Name = "HR"
				};

				session.Save(dept2);

				var dept3 = new Department
				{
					Id = 13,
					Name = "Develop"
				};

				session.Save(dept3);

				var emp = new Employee
				{
					Id = 1,
					FirstName = "John",
					LastName = "Doe",
					Department1 = dept,
					Department2 = dept2,
					Department3 = dept3
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
		public void DepartmentIsNull()
		{
			ExecuteStatement($"UPDATE EMPLOYEES SET DEPARTMENT_ID_1 = 99999 WHERE EMPLOYEE_ID = {employeeId}");

			IStatistics statistics = Sfi.Statistics;
			statistics.Clear();

			using (ISession session = OpenSession())
			{
				var employee = session.Get<Employee>(employeeId);
				Assert.That(employee.Department1, Is.Null);
				Assert.That(employee.Department2, Is.Not.Null);
				Assert.That(employee.Department3, Is.Not.Null);
				Assert.That(statistics.PrepareStatementCount, Is.EqualTo(1));
			}
		}
		
		[Test]
		public void DepartmentIsNotNull()
		{
			ExecuteStatement($"UPDATE EMPLOYEES SET DEPARTMENT_ID_1 = 11 WHERE EMPLOYEE_ID = {employeeId}");

			IStatistics statistics = Sfi.Statistics;
			statistics.Clear();

			using (ISession session = OpenSession())
			{
				var employee = session.Get<Employee>(employeeId);
				Assert.That(employee.Department1, Is.Not.Null);
				Assert.That(employee.Department2, Is.Not.Null);
				Assert.That(employee.Department3, Is.Not.Null);
				Assert.That(statistics.PrepareStatementCount, Is.EqualTo(1));
			}
		}
		
		[Test]
		public void Departmentg12IsNotNull()
		{
			ExecuteStatement($"UPDATE EMPLOYEES SET DEPARTMENT_ID_1 = 11, DEPARTMENT_ID_2 = 99999 WHERE EMPLOYEE_ID = {employeeId}");

			IStatistics statistics = Sfi.Statistics;
			statistics.Clear();

			using (ISession session = OpenSession())
			{
				var employee = session.Get<Employee>(employeeId);
				Assert.That(employee.Department1, Is.Not.Null);
				Assert.That(employee.Department2, Is.Null);
				Assert.That(employee.Department3, Is.Not.Null);
				Assert.That(statistics.PrepareStatementCount, Is.EqualTo(1));
			}
		}
	}
}

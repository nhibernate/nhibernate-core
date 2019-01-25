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

				var address = new Address
				{
					Id = 15,
					Line = "Test"
				};

				session.Save(address);

				var phone1 = new Phone
				{
					Id = 20, 
					Number = "123456"
				};

				session.Save(phone1);
				
				var phone2 = new Phone
				{
					Id = 21, 
					Number = "7891011"
				};

				session.Save(phone2);

				var emp = new Employee
				{
					Id = 1,
					FirstName = "John",
					LastName = "Doe",
					Department1 = dept,
					Department2 = dept2,
					Department3 = dept3,
					Address = address
				};

				emp.Phones.Add(phone1);
				emp.Phones.Add(phone2);

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
				sess.Delete("from Department");
				sess.Delete("from AddressEmployee");
				sess.Delete("from Phone");
				sess.Delete("from Employee");
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
				Assert.That(employee.Address, Is.Not.Null);
				Assert.That(employee.Phones, Is.Not.Null);
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
				Assert.That(employee.Address, Is.Not.Null);
				Assert.That(statistics.PrepareStatementCount, Is.EqualTo(1));
			}
		}

		[Test]
		public void Department2IsNotNull()
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
				Assert.That(employee.Address, Is.Not.Null);
				Assert.That(statistics.PrepareStatementCount, Is.EqualTo(1));
			}
		}

		[Test]
		public void AddressNull()
		{
			ExecuteStatement(
				$"UPDATE EMPLOYEES SET DEPARTMENT_ID_1 = 11, DEPARTMENT_ID_2 = 12, ADDRESS_ID = 99999 WHERE EMPLOYEE_ID = {employeeId}");

			IStatistics statistics = Sfi.Statistics;
			statistics.Clear();

			using (ISession session = OpenSession())
			{
				var employee = session.Get<Employee>(employeeId);
				Assert.That(employee.Department1, Is.Not.Null);
				Assert.That(employee.Department2, Is.Not.Null);
				Assert.That(employee.Department3, Is.Not.Null);
				Assert.That(employee.Address, Is.Null);
				Assert.That(statistics.PrepareStatementCount, Is.EqualTo(1));
			}
		}

		[Test]
		public void PhoneIsNull()
		{
			ExecuteStatement($"UPDATE EMPLOYEES SET DEPARTMENT_ID_1 = 11, DEPARTMENT_ID_2 = 12, ADDRESS_ID = 15 WHERE EMPLOYEE_ID = {employeeId}");
			ExecuteStatement($"UPDATE PHONES SET EMPLOYEE_ID = NULL");

			IStatistics statistics = Sfi.Statistics;
			statistics.Clear();

			using (ISession session = OpenSession())
			{
				var employee = session.Get<Employee>(employeeId);
				Assert.That(employee.Department1, Is.Not.Null);
				Assert.That(employee.Department2, Is.Not.Null);
				Assert.That(employee.Department3, Is.Not.Null);
				Assert.That(employee.Address, Is.Not.Null);
				Assert.That(employee.Phones, Is.Not.Null);
				Assert.That(employee.Phones.Count, Is.Zero);
				Assert.That(statistics.PrepareStatementCount, Is.EqualTo(1));
			}
		}
	}
}

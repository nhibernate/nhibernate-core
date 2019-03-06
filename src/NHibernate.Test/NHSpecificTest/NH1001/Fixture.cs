using System.Linq;
using NHibernate.Cfg;
using NHibernate.Linq;
using NUnit.Framework;

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
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
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
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.Delete("from Department");
				session.Delete("from AddressEmployee");
				session.Delete("from Phone");
				session.Delete("from Employee");
				tx.Commit();
			}
		}

		[Test]
		public void DepartmentIsNull()
		{
			ExecuteStatement($"UPDATE EMPLOYEES SET DEPARTMENT_ID_1 = 99999 WHERE EMPLOYEE_ID = {employeeId}");

			var statistics = Sfi.Statistics;
			statistics.Clear();

			using (var session = OpenSession())
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

			var statistics = Sfi.Statistics;
			statistics.Clear();

			using (var session = OpenSession())
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

			var statistics = Sfi.Statistics;
			statistics.Clear();

			using (var session = OpenSession())
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
			ExecuteStatement($"UPDATE EMPLOYEES SET DEPARTMENT_ID_1 = 11, DEPARTMENT_ID_2 = 12, ADDRESS_ID = 99999 WHERE EMPLOYEE_ID = {employeeId}");

			var statistics = Sfi.Statistics;
			statistics.Clear();

			using (var session = OpenSession())
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

			var statistics = Sfi.Statistics;
			statistics.Clear();

			using (var session = OpenSession())
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
		
		[Test]
		public void Department2IsNull_QueryOver()
		{
			ExecuteStatement($"UPDATE EMPLOYEES SET DEPARTMENT_ID_1 = 11, DEPARTMENT_ID_2 = 99999 WHERE EMPLOYEE_ID = {employeeId}");

			var statistics = Sfi.Statistics;
			statistics.Clear();

			using (var session = OpenSession())
			{
				var employee = session.QueryOver<Employee>()
					.Fetch(SelectMode.Fetch, e => e.Department2)
					.Fetch(SelectMode.Skip, e => e.Department1, e => e.Department3, e => e.Address)
					.Where(e => e.Id == employeeId)
					.SingleOrDefault();
				Assert.That(employee.Department2, Is.Null);
				Assert.That(statistics.PrepareStatementCount, Is.EqualTo(4), "Employee, Department1, Department3, and Address");

				Assert.That(employee.Department1, Is.Not.Null);
				Assert.That(employee.Department3, Is.Not.Null);
				Assert.That(employee.Address, Is.Not.Null);
			}
		}

		[Test]
		public void Department2IsNull_Linq()
		{
			ExecuteStatement($"UPDATE EMPLOYEES SET DEPARTMENT_ID_1 = 11, DEPARTMENT_ID_2 = 99999 WHERE EMPLOYEE_ID = {employeeId}");

			var statistics = Sfi.Statistics;
			statistics.Clear();

			using (var session = OpenSession())
			{
				//NOTE: HQL and Linq ignore fetch="join" in mapping.
				var employee = session.Query<Employee>().Fetch(x => x.Department2).Single();
				Assert.That(statistics.PrepareStatementCount, Is.EqualTo(5), "Employee, Department1, Department3, Address, and Phones");
				Assert.That(employee.Department2, Is.Null);
				Assert.That(statistics.PrepareStatementCount, Is.EqualTo(5), "Employee, Department1, Department3, Address, and Phones");

				Assert.That(employee.Department1, Is.Not.Null);
				Assert.That(employee.Department3, Is.Not.Null);
				Assert.That(employee.Address, Is.Not.Null);
			}
		}

		[Test]
		public void Department2IsNullSkipPhones_QueryOver()
		{
			ExecuteStatement($"UPDATE EMPLOYEES SET DEPARTMENT_ID_1 = 11, DEPARTMENT_ID_2 = 99999 WHERE EMPLOYEE_ID = {employeeId}");

			var statistics = Sfi.Statistics;
			statistics.Clear();

			using (var session = OpenSession())
			{
				var employee = session.QueryOver<Employee>()
					.Fetch(SelectMode.Fetch, e => e.Department2)
					.Fetch(SelectMode.Skip, e => e.Department1, e => e.Department3, e => e.Address, e => e.Phones)
					.Where(e => e.Id == employeeId)
					.SingleOrDefault();
				Assert.That(employee.Department2, Is.Null);
				Assert.That(statistics.PrepareStatementCount, Is.EqualTo(5), "Employee, Department1, Department3, Address, and Phones");

				Assert.That(employee.Department1, Is.Not.Null);
				Assert.That(employee.Department3, Is.Not.Null);
				Assert.That(employee.Address, Is.Not.Null);
			}
		}
	}
}

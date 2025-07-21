using System;
using NUnit.Framework;
using System.Linq;

namespace NHibernate.Test.NHSpecificTest.ManyToManyWithFilter
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private Department _department1;
		private Department _department2;
		private Employee _employee1;
		private Employee _employee2;

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				_department1 = new Department();
				_department2 = new Department();

				_employee1 = new Employee();
				_employee2 = new Employee();

				_employee1.Departments.Add(_department1);
				_employee2.Departments.Add(_department1);
				_employee2.Departments.Add(_department2);

				session.Save(_department1);
				session.Save(_department2);
				session.Save(_employee1);
				session.Save(_employee2);

				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.Delete(_employee1);
				session.Delete(_employee2);
				session.Delete(_department1);
				session.Delete(_department2);

				tx.Commit();
			}
		}

		[Theory]
		public void Querying_Employees_Departments_ManyToMany_With_Filter(bool enableFilter)
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				_department1.DeletedAt = DateTime.UtcNow;
				_department2.DeletedAt = DateTime.UtcNow;

				session.Update(_department1);
				session.Update(_department2);

				tx.Commit();
			}

			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				if (enableFilter)
					session.EnableFilter("NotDeletedFilter");

				var departments = session.Query<Department>();
				var employee2 = session.Get<Employee>(_employee2.Id);

				if (enableFilter)
				{
					Assert.That(departments, Is.Empty);
					Assert.That(employee2.Departments, Is.Empty);
				}
				else
				{
					Assert.That(departments.Count, Is.EqualTo(2));
					Assert.That(employee2.Departments, Has.Count.EqualTo(2));
				}


				tx.Commit();
			}
		}
	}
}

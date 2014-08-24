namespace NHibernate.Test.NHSpecificTest.NH2202
{
	using System.Linq;
	using Criterion;
	using NUnit.Framework;
	
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var emp = new Employee() {EmployeeId = 1, NationalId = 1000};
				emp.Addresses.Add(new EmployeeAddress() { Employee = emp, Type = "Postal" });
				emp.Addresses.Add(new EmployeeAddress() { Employee = emp, Type = "Shipping" });
				s.Save(emp);
				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete("from EmployeeAddress");
				tx.Commit();
			}

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete("from Employee");
				tx.Commit();
			}

			base.OnTearDown();
		}

		[Test]
		public void CanProjectEmployeeFromAddressUsingCriteria()
		{
			using (var s = OpenSession())
			{
				var employees = s.CreateCriteria<EmployeeAddress>("x3")
					.Add(Restrictions.Eq("Type", "Postal"))
					.SetProjection(Projections.Property("Employee"))
					.List<Employee>();

				Assert.That(employees.FirstOrDefault(), Is.InstanceOf<Employee>());
			}
		}
	}
}
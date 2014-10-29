using System.Linq;
using NHibernate.Cfg;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Linq;
using NHibernate.Test.Linq;
using NUnit.Framework;

namespace NHibernate.Test.DriverTest
{
	[TestFixture]
	public class BulkInsertTests : LinqTestCase
	{
		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.Hbm2ddlAuto, SchemaAutoAction.Create.ToString());

			base.Configure(configuration);
		}

		[Test]
		public void CanBulkInsertEntitiesWithComponents()
		{
			using (session.BeginTransaction())
			{
				var customers = new Customer[] { new Customer { Address = new Address("street", "city", "region", "postalCode", "country", "phoneNumber", "fax"), CompanyName = "Company", ContactName = "Contact", ContactTitle = "Title", CustomerId = "12345" } };

				this.session.CreateQuery("delete from Customer").ExecuteUpdate();

				this.session.BulkInsert(customers);

				var count = this.session.Query<Customer>().Count();

				Assert.AreEqual(customers.Count(), count);
			}
		}

		[Test]
		public void CanBulkInsertEntitiesWithComponentsAndAssociations()
		{
			using (session.BeginTransaction())
			{
				var superior = new Employee { Address = new Address("street", "city", "region", "zip", "country", "phone", "fax"), BirthDate = System.DateTime.Now, EmployeeId = 1, Extension = "1", FirstName = "Superior", LastName = "Last" };
				var employee = new Employee { Address = new Address("street", "city", "region", "zip", "country", "phone", "fax"), BirthDate = System.DateTime.Now, EmployeeId = 2, Extension = "2", FirstName = "Employee", LastName = "Last", Superior = superior };
				var employees = new Employee[] { superior, employee };

				this.session.CreateQuery("delete from Employee").ExecuteUpdate();

				this.session.BulkInsert(employees);

				var count = this.session.Query<Employee>().Count();

				Assert.AreEqual(employees.Count(), count);
			}
		}
	}
}

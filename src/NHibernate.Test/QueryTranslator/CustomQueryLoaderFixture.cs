using System.Linq;
using NHibernate.Cfg;
using NHibernate.DomainModel.Northwind.Entities;
using NUnit.Framework;

namespace NHibernate.Test.QueryTranslator
{
	[TestFixture]
	internal sealed class CustomQueryLoaderFixture : TestCase
	{
		private ISession _session;
		private ITransaction _transaction;

		protected override string[] Mappings => new[]
		                                        {
			                                        "Northwind.Mappings.Customer.hbm.xml",
			                                        "Northwind.Mappings.Employee.hbm.xml",
			                                        "Northwind.Mappings.Order.hbm.xml",
			                                        "Northwind.Mappings.OrderLine.hbm.xml",
			                                        "Northwind.Mappings.Product.hbm.xml",
			                                        "Northwind.Mappings.ProductCategory.hbm.xml",
			                                        "Northwind.Mappings.Region.hbm.xml",
			                                        "Northwind.Mappings.Shipper.hbm.xml",
			                                        "Northwind.Mappings.Supplier.hbm.xml",
			                                        "Northwind.Mappings.Territory.hbm.xml",
			                                        "Northwind.Mappings.AnotherEntity.hbm.xml",
			                                        "Northwind.Mappings.Role.hbm.xml",
			                                        "Northwind.Mappings.User.hbm.xml",
			                                        "Northwind.Mappings.TimeSheet.hbm.xml",
			                                        "Northwind.Mappings.Animal.hbm.xml",
			                                        "Northwind.Mappings.Patient.hbm.xml",
			                                        "Northwind.Mappings.NumericEntity.hbm.xml"
		                                        };

		protected override string MappingsAssembly => "NHibernate.DomainModel";

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.QueryTranslator, typeof(CustomQueryTranslatorFactory).AssemblyQualifiedName);
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
			
			_session = OpenSession();
			_transaction = _session.BeginTransaction();

			var customer = new Customer
			               {
				               CustomerId = "C1",
				               CompanyName = "Company"
			               };
			_session.Save(customer);
			_session.Flush();
			_session.Clear();
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			
			_transaction.Rollback();
			_transaction.Dispose();
			_session.Close();
			_session.Dispose();
		}

		[Test]
		public void CriteriaQueryTest()
		{
			var customers = _session.CreateCriteria(typeof(Customer))
			                        .List<Customer>();
			
			Assert.AreEqual(1, customers.Count);
		}
		
		[Test]
		public void HqlQueryTest()
		{
			var customers = _session.CreateQuery("select c from Customer c")
			                        .List<Customer>();
			
			Assert.AreEqual(1, customers.Count);
		}

		[Test]
		public void LinqQueryTest()
		{
			var customers = _session.Query<Customer>()
			                        .ToList();
			
			Assert.AreEqual(1, customers.Count);
		}

		[Test]
		public void QueryOverQueryTest()
		{
			var customers = _session.QueryOver<Customer>()
			                        .List<Customer>();
			
			Assert.AreEqual(1, customers.Count);
		}
	}
}

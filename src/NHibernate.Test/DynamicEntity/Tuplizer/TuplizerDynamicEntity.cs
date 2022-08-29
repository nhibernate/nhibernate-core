using System;
using System.Collections.Generic;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.DynamicEntity.Tuplizer
{
	[TestFixture]
	[Obsolete("Require dynamic proxies")]
	public class TuplizerDynamicEntity : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override string[] Mappings
		{
			get { return new string[] { "DynamicEntity.Tuplizer.Customer.hbm.xml" }; }
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.SetInterceptor(new EntityNameInterceptor());
		}

		[Test]
		public void It()
		{
			var company = ProxyHelper.NewCompanyProxy();
			var customer = ProxyHelper.NewCustomerProxy();
			var address = ProxyHelper.NewAddressProxy();
			var son = ProxyHelper.NewPersonProxy();
			var wife = ProxyHelper.NewPersonProxy();

			// Test saving these dyna-proxies
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				company.Name = "acme";
				session.Save(company);
				customer.Name = "Steve";
				customer.Company = company;
				address.Street = "somewhere over the rainbow";
				address.City = "lawerence, kansas";
				address.PostalCode = "toto";
				customer.Address = address;
				customer.Family = new HashSet<Person>();
				son.Name = "son";
				customer.Family.Add(son);
				wife.Name = "wife";
				customer.Family.Add(wife);
				session.Save(customer);
				tran.Commit();
				session.Close();
			}

			Assert.IsNotNull(company.Id, "company id not assigned");
			Assert.IsNotNull(customer.Id, "customer id not assigned");
			Assert.IsNotNull(address.Id, "address id not assigned");
			Assert.IsNotNull(son.Id, "son:Person id not assigned");
			Assert.IsNotNull(wife.Id, "wife:Person id not assigned");

			// Test loading these dyna-proxies, along with flush processing
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				customer = session.Load<Customer>(customer.Id);
				Assert.IsFalse(NHibernateUtil.IsInitialized(customer), "should-be-proxy was initialized");

				customer.Name = "other";
				session.Flush();
				Assert.IsFalse(NHibernateUtil.IsInitialized(customer.Company), "should-be-proxy was initialized");

				session.Refresh(customer);
				Assert.AreEqual("other", customer.Name, "name not updated");
				Assert.AreEqual("acme", customer.Company.Name, "company association not correct");

				tran.Commit();
				session.Close();
			}

			// Test detached entity re-attachment with these dyna-proxies
			customer.Name = "Steve";
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				session.Update(customer);
				session.Flush();
				session.Refresh(customer);
				Assert.AreEqual("Steve", customer.Name, "name not updated");
				tran.Commit();
				session.Close();
			}

			// Test querying
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				int count = session.CreateQuery("from Customer").List().Count;
				Assert.AreEqual(1, count, "querying dynamic entity");
				session.Clear();
				count = session.CreateQuery("from Person").List().Count;
				Assert.AreEqual(3, count, "querying dynamic entity");
				tran.Commit();
				session.Close();
			}

			// test deleteing
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				session.Delete(company);
				session.Delete(customer);
				tran.Commit();
				session.Close();
			}
		}
	}
}

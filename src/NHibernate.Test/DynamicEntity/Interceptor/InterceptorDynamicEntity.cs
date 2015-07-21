using System.Collections;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.DynamicEntity.Interceptor
{
	[TestFixture]
	public class InterceptorDynamicEntity : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] {"DynamicEntity.Interceptor.Customer.hbm.xml"}; }
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.SetInterceptor(new ProxyInterceptor());
		}

		[Test]
		public void It()
		{
			// Test saving these dyna-proxies
			ISession session = OpenSession();
			session.BeginTransaction();
			Company company = ProxyHelper.NewCompanyProxy();
			company.Name = "acme";
			session.Save(company);
			Customer customer = ProxyHelper.NewCustomerProxy();
			customer.Name = "Steve";
			customer.Company = company;
			session.Save(customer);
			session.Transaction.Commit();
			session.Close();

			Assert.IsNotNull(company.Id, "company id not assigned");
			Assert.IsNotNull(customer.Id, "customer id not assigned");

			// Test loading these dyna-proxies, along with flush processing
			session = OpenSession();
			session.BeginTransaction();
			customer = session.Load<Customer>(customer.Id);
			Assert.IsFalse(NHibernateUtil.IsInitialized(customer), "should-be-proxy was initialized");

			customer.Name = "other";
			session.Flush();
			Assert.IsFalse(NHibernateUtil.IsInitialized(customer.Company), "should-be-proxy was initialized");

			session.Refresh(customer);
			Assert.AreEqual("other", customer.Name, "name not updated");
			Assert.AreEqual("acme", customer.Company.Name, "company association not correct");

			session.Transaction.Commit();
			session.Close();

			// Test detached entity re-attachment with these dyna-proxies
			customer.Name = "Steve";
			session = OpenSession();
			session.BeginTransaction();
			session.Update(customer);
			session.Flush();
			session.Refresh(customer);
			Assert.AreEqual("Steve", customer.Name, "name not updated");
			session.Transaction.Commit();
			session.Close();

			// Test querying
			session = OpenSession();
			session.BeginTransaction();
			int count = session.CreateQuery("from Customer").List().Count;
			Assert.AreEqual(1, count, "querying dynamic entity");
			session.Clear();
			count = session.CreateQuery("from Person").List().Count;
			Assert.AreEqual(1, count, "querying dynamic entity");
			session.Transaction.Commit();
			session.Close();

			// test deleteing
			session = OpenSession();
			session.BeginTransaction();
			session.Delete(company);
			session.Delete(customer);
			session.Transaction.Commit();
			session.Close();
		}
	}
}
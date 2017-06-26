using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.TypedManyToOne
{
	[TestFixture]
	public class TypedManyToOneTest : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] { "TypedManyToOne.Customer.hbm.xml" }; }
		}

		[Test]
		public void TestCreateQuery()
		{
			var cust = new Customer();
			cust.CustomerId = "abc123";
			cust.Name = "Matt";

			var ship = new Address();
			ship.Street = "peachtree rd";
			ship.State = "GA";
			ship.City = "ATL";
			ship.Zip = "30326";
			ship.AddressId = new AddressId("SHIPPING", "xyz123");
			ship.Customer = cust;

			var bill = new Address();
			bill.Street = "peachtree rd";
			bill.State = "GA";
			bill.City = "ATL";
			bill.Zip = "30326";
			bill.AddressId = new AddressId("BILLING", "xyz123");
			bill.Customer = cust;

			cust.BillingAddress = bill;
			cust.ShippingAddress = ship;

			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Persist(cust);
				t.Commit();
			}

			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				IList results = s.CreateQuery("from Customer cust left join fetch cust.BillingAddress where cust.CustomerId='abc123'").List();
				//IList results = s.CreateQuery("from Customer cust left join fetch cust.BillingAddress left join fetch cust.ShippingAddress").List();
				cust = (Customer)results[0];
				Assert.That(NHibernateUtil.IsInitialized(cust.ShippingAddress), Is.False);
				Assert.That(NHibernateUtil.IsInitialized(cust.BillingAddress), Is.True);
				Assert.That(cust.BillingAddress.Zip, Is.EqualTo("30326"));
				Assert.That(cust.ShippingAddress.Zip, Is.EqualTo("30326"));
				Assert.That(cust.BillingAddress.AddressId.Type, Is.EqualTo("BILLING"));
				Assert.That(cust.ShippingAddress.AddressId.Type, Is.EqualTo("SHIPPING"));
				t.Commit();
			}

			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.SaveOrUpdate(cust);
				ship = cust.ShippingAddress;
				cust.ShippingAddress = null;
				s.Delete("ShippingAddress", ship);
				s.Flush();

				Assert.That(s.Get("ShippingAddress", ship.AddressId), Is.Null);
				s.Delete(cust);

				t.Commit();
			}
		}

		[Test]
		public void TestCreateQueryNull()
		{
			var cust = new Customer();
			cust.CustomerId = "xyz123";
			cust.Name = "Matt";

			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Persist(cust);
				t.Commit();
			}

			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				IList results = s.CreateQuery("from Customer cust left join fetch cust.BillingAddress where cust.CustomerId='xyz123'").List();
				//IList results = s.CreateQuery("from Customer cust left join fetch cust.BillingAddress left join fetch cust.ShippingAddress").List();
				cust = (Customer)results[0];
				Assert.That(cust.ShippingAddress, Is.Null);
				Assert.That(cust.BillingAddress, Is.Null);
				s.Delete(cust);
				t.Commit();
			}
		}
	}
}

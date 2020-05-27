﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections;
using System.Linq;
using NHibernate.Dialect;
using NUnit.Framework;
using NHibernate.Linq;

namespace NHibernate.Test.TypedManyToOne
{
	using System.Threading.Tasks;
	using System.Threading;
	[TestFixture]
	public class TypedManyToOneTestAsync : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override string[] Mappings
		{
			get { return new[] { "TypedManyToOne.Customer.hbm.xml" }; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			// Mapping uses check constraint, which Ms SQL CE does not support.
			return !(Dialect is MsSqlCeDialect);
		}

		[Test]
		public async Task TestLinqEntityNameQueryAsync()
		{
			var cust = await (CreateCustomerAsync());
			using (var s = Sfi.OpenSession())
			using (var t = s.BeginTransaction())
			{
				var billingNotes = await (s.Query<Customer>().Select(o => o.BillingAddress.BillingNotes).FirstAsync());
				Assert.That(billingNotes, Is.EqualTo("BillingNotes"));
				var shippingNotes = await (s.Query<Customer>().Select(o => o.ShippingAddress.ShippingNotes).FirstAsync());
				Assert.That(shippingNotes, Is.EqualTo("ShippingNotes"));

				await (t.CommitAsync());
			}

			await (DeleteCustomerAsync(cust));
		}

		[Test]
		public async Task TestCreateQueryAsync()
		{
			var cust = await (CreateCustomerAsync());
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				IList results = await (s.CreateQuery("from Customer cust left join fetch cust.BillingAddress where cust.CustomerId='abc123'").ListAsync());
				//IList results = s.CreateQuery("from Customer cust left join fetch cust.BillingAddress left join fetch cust.ShippingAddress").List();
				cust = (Customer)results[0];
				Assert.That(NHibernateUtil.IsInitialized(cust.ShippingAddress), Is.False);
				Assert.That(NHibernateUtil.IsInitialized(cust.BillingAddress), Is.True);
				Assert.That(cust.BillingAddress.Zip, Is.EqualTo("30326"));
				Assert.That(cust.ShippingAddress.Zip, Is.EqualTo("30326"));
				Assert.That(cust.BillingAddress.AddressId.Type, Is.EqualTo("BILLING"));
				Assert.That(cust.ShippingAddress.AddressId.Type, Is.EqualTo("SHIPPING"));
				await (t.CommitAsync());
			}

			await (DeleteCustomerAsync(cust));
		}

		[Test]
		public async Task TestCreateQueryNullAsync()
		{
			var cust = new Customer();
			cust.CustomerId = "xyz123";
			cust.Name = "Matt";

			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				await (s.PersistAsync(cust));
				await (t.CommitAsync());
			}

			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				IList results = await (s.CreateQuery("from Customer cust left join fetch cust.BillingAddress where cust.CustomerId='xyz123'").ListAsync());
				//IList results = s.CreateQuery("from Customer cust left join fetch cust.BillingAddress left join fetch cust.ShippingAddress").List();
				cust = (Customer)results[0];
				Assert.That(cust.ShippingAddress, Is.Null);
				Assert.That(cust.BillingAddress, Is.Null);
				await (s.DeleteAsync(cust));
				await (t.CommitAsync());
			}
		}

		private async Task<Customer> CreateCustomerAsync(CancellationToken cancellationToken = default(CancellationToken))
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
			ship.ShippingNotes = "ShippingNotes";

			var bill = new Address();
			bill.Street = "peachtree rd";
			bill.State = "GA";
			bill.City = "ATL";
			bill.Zip = "30326";
			bill.AddressId = new AddressId("BILLING", "xyz123");
			bill.Customer = cust;
			bill.BillingNotes = "BillingNotes";

			cust.BillingAddress = bill;
			cust.ShippingAddress = ship;

			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				await (s.PersistAsync(cust, cancellationToken));
				await (t.CommitAsync(cancellationToken));
			}

			return cust;
		}

		private async Task DeleteCustomerAsync(Customer cust, CancellationToken cancellationToken = default(CancellationToken))
		{
			using (var s = Sfi.OpenSession())
			using (var t = s.BeginTransaction())
			{
				await (s.SaveOrUpdateAsync(cust, cancellationToken));
				var ship = cust.ShippingAddress;
				cust.ShippingAddress = null;
				await (s.DeleteAsync("ShippingAddress", ship, cancellationToken));
				await (s.FlushAsync(cancellationToken));

				Assert.That(await (s.GetAsync("ShippingAddress", ship.AddressId, cancellationToken)), Is.Null);
				await (s.DeleteAsync(cust, cancellationToken));

				await (t.CommitAsync(cancellationToken));
			}
		}
	}
}

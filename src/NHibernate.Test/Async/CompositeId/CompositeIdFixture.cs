﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;
using System.Linq;
using NHibernate.Dialect;
using NUnit.Framework;
using NHibernate.Linq;

namespace NHibernate.Test.CompositeId
{
	using System.Threading.Tasks;
	using System.Threading;
	[TestFixture]
	public class CompositeIdFixtureAsync : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override string[] Mappings
		{
			get
			{
				return new string[]
				       	{
				       		"CompositeId.Customer.hbm.xml", "CompositeId.Order.hbm.xml", "CompositeId.LineItem.hbm.xml",
				       		"CompositeId.Product.hbm.xml", "CompositeId.Shipper.hbm.xml"
				       	};
			}
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			// Order uses a scalar sub-select formula.
			return Dialect.SupportsScalarSubSelects;
		}

		[Test]
		public async Task CompositeIdsAsync()
		{
			ISession s;
			ITransaction t;
			Product p2;
			using (s = OpenSession())
			{
				t = s.BeginTransaction();

				Product p = new Product();
				p.ProductId = "A123";
				p.Description = "nipple ring";
				p.Price = 1.0m;
				p.NumberAvailable = 1004;
				await (s.PersistAsync(p));

				p2 = new Product();
				p2.ProductId = "X525";
				p2.Description = "nose stud";
				p2.Price = 3.0m;
				p2.NumberAvailable = 105;
				await (s.PersistAsync(p2));

				Customer c = new Customer();
				c.Address = "St Kilda Rd, MEL, 3000";
				c.Name = "Virginia";
				c.CustomerId = "C111";
				await (s.PersistAsync(c));

				Order o = new Order(c);
				o.OrderDate = DateTime.Today;
				o.Shipper = new Shipper() { Id = new NullableId(null, 13) };
				await (s.PersistAsync(o));
				
				LineItem li = new LineItem(o, p);
				li.Quantity = 2;
				await (s.PersistAsync(li));
				
				await (t.CommitAsync());
			}

			using (s = OpenSession())
			{
				t = s.BeginTransaction();
				Order o = await (s.GetAsync<Order>(new Order.ID("C111", 0)));
				Assert.That(o.Total == 2m);
				await (t.CommitAsync());
			}

			using(s = OpenSession())
			{
				t = s.BeginTransaction();
				await (s.CreateQuery(
					"from Customer c left join fetch c.Orders o left join fetch o.LineItems li left join fetch li.Product p").ListAsync());
				await (t.CommitAsync());
			}

			using(s = OpenSession())
			{
				t = s.BeginTransaction();
				await (s.CreateQuery("from Order o left join fetch o.LineItems li left join fetch li.Product p").ListAsync());
				await (t.CommitAsync());
			}

			using(s = OpenSession())
			{
				t = s.BeginTransaction();
				IEnumerable iter = await (s.CreateQuery("select o.id, li.id from Order o join o.LineItems li").ListAsync());
				foreach (object[] stuff in iter)
				{
					Assert.AreEqual(2, stuff.Length);
				}
				iter = await (s.CreateQuery("from Order o join o.LineItems li").EnumerableAsync());
				foreach (object[] stuff in iter)
				{
					Assert.AreEqual(2, stuff.Length);
				}
				await (t.CommitAsync());
			}

			using(s = OpenSession())
			{
				t = s.BeginTransaction();
				Customer c = await (s.GetAsync<Customer>("C111"));
				Order o2 = new Order(c);
				o2.OrderDate = DateTime.Today;
				await (s.FlushAsync());
				LineItem li2 = new LineItem(o2, p2);
				li2.Quantity = 5;
				IList bigOrders = await (s.CreateQuery("from Order o where o.Total>10.0").ListAsync());
				Assert.AreEqual(1, bigOrders.Count);
				await (t.CommitAsync());
			}

			using (s = OpenSession())
			{
				t = s.BeginTransaction();
				var noShippersForWarehouse = await (s.Query<Order>()
					// NOTE: .Where(x => x.Shipper.Id == new NullableId(null, 13)) improperly renders
					// "where (ShipperId = @p1 and WarehouseId = @p2)" with @p1 = NULL (needs to be is null)
					// But the effort to fix is pretty high due to how component tuples are managed in linq / hql.
					.Where(x => x.Shipper.Id.WarehouseId == 13 && x.Shipper.Id.Id == null)
					.ToListAsync());
				Assert.AreEqual(1, noShippersForWarehouse.Count);
				await (t.CommitAsync());
			}

			using (s = OpenSession())
			{
				t = s.BeginTransaction();
				await (s.DeleteAsync("from LineItem"));
				await (s.DeleteAsync("from Order"));
				await (s.DeleteAsync("from Customer"));
				await (s.DeleteAsync("from Product"));
				await (t.CommitAsync());
			}
		}

		[Test]
		public async Task MultipleCollectionFetchAsync()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Product p = new Product();
			p.ProductId = "A123";
			p.Description = "nipple ring";
			p.Price = 1.0m;
			p.NumberAvailable = 1004;
			await (s.PersistAsync(p));

			Product p2 = new Product();
			p2.ProductId = "X525";
			p2.Description = "nose stud";
			p2.Price = 3.0m;
			p2.NumberAvailable = 105;
			await (s.PersistAsync(p2));

			Customer c = new Customer();
			c.Address = "St Kilda Rd, MEL, 3000";
			c.Name = "Virginia";
			c.CustomerId = "C111";
			await (s.PersistAsync(c));

			Order o = new Order(c);
			o.OrderDate = DateTime.Today;
			LineItem li = new LineItem(o, p);
			li.Quantity = 2;
			LineItem li2 = new LineItem(o, p2);
			li2.Quantity = 3;

			Order o2 = new Order(c);
			o2.OrderDate = DateTime.Today;
			LineItem li3 = new LineItem(o2, p);
			li3.Quantity = 1;
			LineItem li4 = new LineItem(o2, p2);
			li4.Quantity = 1;

			await (t.CommitAsync());
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			c =
				(Customer)
				await (s.CreateQuery(
					"from Customer c left join fetch c.Orders o left join fetch o.LineItems li left join fetch li.Product p").
					UniqueResultAsync());
			Assert.IsTrue(NHibernateUtil.IsInitialized(c.Orders));
			Assert.AreEqual(2, c.Orders.Count);
			Assert.IsTrue(NHibernateUtil.IsInitialized(((Order) c.Orders[0]).LineItems));
			Assert.IsTrue(NHibernateUtil.IsInitialized(((Order) c.Orders[1]).LineItems));
			Assert.AreEqual(2, ((Order) c.Orders[0]).LineItems.Count);
			Assert.AreEqual(2, ((Order) c.Orders[1]).LineItems.Count);
			await (t.CommitAsync());
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			await (s.DeleteAsync("from LineItem"));
			await (s.DeleteAsync("from Order"));
			await (s.DeleteAsync("from Customer"));
			await (s.DeleteAsync("from Product"));
			await (t.CommitAsync());
			s.Close();
		}

		[Test]
		public async Task NonLazyFetchAsync()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Product p = new Product();
			p.ProductId = "A123";
			p.Description = "nipple ring";
			p.Price = 1.0m;
			p.NumberAvailable = 1004;
			await (s.PersistAsync(p));

			Product p2 = new Product();
			p2.ProductId = "X525";
			p2.Description = "nose stud";
			p2.Price = 3.0m;
			p2.NumberAvailable = 105;
			await (s.PersistAsync(p2));

			Customer c = new Customer();
			c.Address = "St Kilda Rd, MEL, 3000";
			c.Name = "Virginia";
			c.CustomerId = "C111";
			await (s.PersistAsync(c));

			Order o = new Order(c);
			o.OrderDate = DateTime.Today;
			LineItem li = new LineItem(o, p);
			li.Quantity = 2;

			await (t.CommitAsync());
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			o = await (s.GetAsync<Order>(new Order.ID("C111", 0)));
			Assert.AreEqual(2m, o.Total);
			await (t.CommitAsync());
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			o = (Order) await (s.CreateQuery("from Order o left join fetch o.LineItems li left join fetch li.Product p").UniqueResultAsync());
			Assert.IsTrue(NHibernateUtil.IsInitialized(o.LineItems));
			li = (LineItem) o.LineItems[0];
			Assert.IsTrue(NHibernateUtil.IsInitialized(li));
			Assert.IsTrue(NHibernateUtil.IsInitialized(li.Product));
			await (t.CommitAsync());
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			o = (Order) await (s.CreateQuery("from Order o").UniqueResultAsync());
			Assert.IsTrue(NHibernateUtil.IsInitialized(o.LineItems));
			li = (LineItem) o.LineItems[0];
			Assert.IsTrue(NHibernateUtil.IsInitialized(li));
			Assert.IsFalse(NHibernateUtil.IsInitialized(li.Product));
			await (t.CommitAsync());
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			await (s.DeleteAsync("from LineItem"));
			await (s.DeleteAsync("from Order"));
			await (s.DeleteAsync("from Customer"));
			await (s.DeleteAsync("from Product"));
			await (t.CommitAsync());
			s.Close();
		}

		[Test]
		public async Task QueryAsync()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			await (s.CreateQuery("from LineItem ol where ol.Order.Id.CustomerId = 'C111'").ListAsync());
			await (t.CommitAsync());
			s.Close();
		}

		[Test(Description = "GH-2646")]
		public async Task AnyOnCompositeIdAsync()
		{
			using (var s = OpenSession())
			{
				await (s.Query<Order>().Where(o => o.LineItems.Any()).ToListAsync());
				await (s.Query<Order>().Select(o => o.LineItems.Any()).ToListAsync());
			}
		}

		public async Task NullCompositeIdAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			using (var s = OpenSession())
			{
				await (s.Query<Order>().Where(o => o.LineItems.Any()).ToListAsync(cancellationToken));
				await (s.Query<Order>().Select(o => o.LineItems.Any()).ToListAsync(cancellationToken));
			}
		}
	}
}

using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.CompositeId
{
	[TestFixture]
	public class CompositeIdFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get
			{
				return new string[]
				       	{
				       		"CompositeId.Customer.hbm.xml", "CompositeId.Order.hbm.xml", "CompositeId.LineItem.hbm.xml",
				       		"CompositeId.Product.hbm.xml"
				       	};
			}
		}

		protected override string CacheConcurrencyStrategy
		{
			get { return null; }
		}

		[Test]
		public void CompositeIds()
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
				s.Persist(p);

				p2 = new Product();
				p2.ProductId = "X525";
				p2.Description = "nose stud";
				p2.Price = 3.0m;
				p2.NumberAvailable = 105;
				s.Persist(p2);

				Customer c = new Customer();
				c.Address = "St Kilda Rd, MEL, 3000";
				c.Name = "Virginia";
				c.CustomerId = "C111";
				s.Persist(c);

				Order o = new Order(c);
				o.OrderDate = DateTime.Today;
				LineItem li = new LineItem(o, p);
				li.Quantity = 2;

				t.Commit();
			}

			using (s = OpenSession())
			{
				t = s.BeginTransaction();
				Order o = s.Get<Order>(new Order.ID("C111", 0));
				Assert.That(o.Total == 2m);
				t.Commit();
			}

			using(s = OpenSession())
			{
				t = s.BeginTransaction();
				s.CreateQuery(
					"from Customer c left join fetch c.Orders o left join fetch o.LineItems li left join fetch li.Product p").List();
				t.Commit();
			}

			using(s = OpenSession())
			{
				t = s.BeginTransaction();
				s.CreateQuery("from Order o left join fetch o.LineItems li left join fetch li.Product p").List();
				t.Commit();
			}

			using(s = OpenSession())
			{
				t = s.BeginTransaction();
				IEnumerable iter = s.CreateQuery("select o.id, li.id from Order o join o.LineItems li").List();
				foreach (object[] stuff in iter)
				{
					Assert.AreEqual(2, stuff.Length);
				}
				iter = s.CreateQuery("from Order o join o.LineItems li").Enumerable();
				foreach (object[] stuff in iter)
				{
					Assert.AreEqual(2, stuff.Length);
				}
				t.Commit();
			}

			using(s = OpenSession())
			{
				t = s.BeginTransaction();
				Customer c = s.Get<Customer>("C111");
				Order o2 = new Order(c);
				o2.OrderDate = DateTime.Today;
				s.Flush();
				LineItem li2 = new LineItem(o2, p2);
				li2.Quantity = 5;
				IList bigOrders = s.CreateQuery("from Order o where o.Total>10.0").List();
				Assert.AreEqual(1, bigOrders.Count);
				t.Commit();
			}

			
			using (s = OpenSession())
			{
				t = s.BeginTransaction();
				s.Delete("from LineItem");
				s.Delete("from Order");
				s.Delete("from Customer");
				s.Delete("from Product");
				t.Commit();
			}
		}

		[Test]
		public void MultipleCollectionFetch()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Product p = new Product();
			p.ProductId = "A123";
			p.Description = "nipple ring";
			p.Price = 1.0m;
			p.NumberAvailable = 1004;
			s.Persist(p);

			Product p2 = new Product();
			p2.ProductId = "X525";
			p2.Description = "nose stud";
			p2.Price = 3.0m;
			p2.NumberAvailable = 105;
			s.Persist(p2);

			Customer c = new Customer();
			c.Address = "St Kilda Rd, MEL, 3000";
			c.Name = "Virginia";
			c.CustomerId = "C111";
			s.Persist(c);

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

			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			c =
				(Customer)
				s.CreateQuery(
					"from Customer c left join fetch c.Orders o left join fetch o.LineItems li left join fetch li.Product p").
					UniqueResult();
			Assert.IsTrue(NHibernateUtil.IsInitialized(c.Orders));
			Assert.AreEqual(2, c.Orders.Count);
			Assert.IsTrue(NHibernateUtil.IsInitialized(((Order) c.Orders[0]).LineItems));
			Assert.IsTrue(NHibernateUtil.IsInitialized(((Order) c.Orders[1]).LineItems));
			Assert.AreEqual(((Order) c.Orders[0]).LineItems.Count, 2);
			Assert.AreEqual(((Order) c.Orders[1]).LineItems.Count, 2);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			s.Delete("from LineItem");
			s.Delete("from Order");
			s.Delete("from Customer");
			s.Delete("from Product");
			t.Commit();
			s.Close();
		}

		[Test]
		public void NonLazyFetch()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Product p = new Product();
			p.ProductId = "A123";
			p.Description = "nipple ring";
			p.Price = 1.0m;
			p.NumberAvailable = 1004;
			s.Persist(p);

			Product p2 = new Product();
			p2.ProductId = "X525";
			p2.Description = "nose stud";
			p2.Price = 3.0m;
			p2.NumberAvailable = 105;
			s.Persist(p2);

			Customer c = new Customer();
			c.Address = "St Kilda Rd, MEL, 3000";
			c.Name = "Virginia";
			c.CustomerId = "C111";
			s.Persist(c);

			Order o = new Order(c);
			o.OrderDate = DateTime.Today;
			LineItem li = new LineItem(o, p);
			li.Quantity = 2;

			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			o = s.Get<Order>(new Order.ID("C111", 0));
			Assert.AreEqual(2m, o.Total);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			o = (Order) s.CreateQuery("from Order o left join fetch o.LineItems li left join fetch li.Product p").UniqueResult();
			Assert.IsTrue(NHibernateUtil.IsInitialized(o.LineItems));
			li = (LineItem) o.LineItems[0];
			Assert.IsTrue(NHibernateUtil.IsInitialized(li));
			Assert.IsTrue(NHibernateUtil.IsInitialized(li.Product));
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			o = (Order) s.CreateQuery("from Order o").UniqueResult();
			Assert.IsTrue(NHibernateUtil.IsInitialized(o.LineItems));
			li = (LineItem) o.LineItems[0];
			Assert.IsTrue(NHibernateUtil.IsInitialized(li));
			Assert.IsFalse(NHibernateUtil.IsInitialized(li.Product));
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			s.Delete("from LineItem");
			s.Delete("from Order");
			s.Delete("from Customer");
			s.Delete("from Product");
			t.Commit();
			s.Close();
		}

		[Test]
		public void Query()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.CreateQuery("from LineItem ol where ol.Order.Id.CustomerId = 'C111'").List();
			t.Commit();
			s.Close();
		}
	}
}
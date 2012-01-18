using System;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq.ByMethod
{
	[TestFixture]
	public class GroupByHavingTests : LinqTestCase
	{
		protected override void Configure(Cfg.Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SetProperty(Cfg.Environment.ShowSql, "true");
		} 

		[Test]
		public void HavingCountSelectCount()
		{
			var list = (from o in db.Orders
						group o by o.OrderDate
						into g
						where g.Count() > 4
						select g.Count()).ToList();

			Assert.That(list.Count, Is.EqualTo(1));
			Assert.That(list.Single(), Is.EqualTo(5));
		}

		[Test]
		public void HavingCountSelectCountWithInnerWhere()
		{
			var list = (from o in db.Orders
						where o.RequiredDate < DateTime.Now
						group o by o.OrderDate
						into g
						where g.Count() > 4
						select g.Count()).ToList();

			Assert.That(list.Count, Is.EqualTo(1));
			Assert.That(list.Single(), Is.EqualTo(5));
		}

		[Test]
		public void HavingCountSelectKey()
		{
			var list = (from o in db.Orders
						group o by o.OrderDate
						into g
						where g.Count() > 4
						select g.Key).ToList();

			Assert.That(list.Count, Is.EqualTo(1));
		}

		[Test]
		public void HavingCountSelectKeyWithInnerWhere()
		{
			var list = (from o in db.Orders
						where o.RequiredDate < DateTime.Now
						group o by o.OrderDate
						into g
						where g.Count() > 4
						select g.Key).ToList();

			Assert.That(list.Count, Is.EqualTo(1));
		}

		[Test]
		public void HavingCountSelectTupleKeyCount()
		{
			var list = (from o in db.Orders
						group o by o.OrderDate
						into g
						where g.Count() > 4
						select new {g.Key, Count = g.Count()}).ToList();

			Assert.That(list.Count, Is.EqualTo(1));
			Assert.That(list.Single().Count, Is.EqualTo(5));
		}

		[Test]
		public void HavingCountSelectTupleKeyCountOfOrders()
		{
			var list = (from o in db.Orders
						group o by o.OrderDate
						into g
						where g.Count() > 4
						select new {g.Key, Count = g.Select(x => x.OrderId).Count()}).ToList();

			Assert.That(list.Count, Is.EqualTo(1));
			Assert.That(list.Single().Count, Is.EqualTo(5));
		}

		[Test]
		public void HavingCountSelectTupleKeyCountOfOrderLines()
		{
			var list = (from o in db.Orders
						group o by o.OrderDate
						into g
						where g.Count() > 4
						select new
								   {
									   g.Key,
									   Count = g.SelectMany(x => x.OrderLines).Count()
								   }).ToList();

			Assert.That(list.Count, Is.EqualTo(1));
			Assert.That(list.Single().Count, Is.EqualTo(5));
		}

		[Test]
		public void ComplexQuery()
		{
			var list = (from o in db.Orders
						where o.RequiredDate < DateTime.Now
						group o by o.OrderDate
						into g
						where g.Count() > 4 && g.Key < DateTime.Now
						select g.Count()).ToList();

			Assert.AreEqual(1, list.Count);
		}
	}
}

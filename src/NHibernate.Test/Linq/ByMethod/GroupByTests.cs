﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NHibernate.Cfg;
using NHibernate.DomainModel.Northwind.Entities;
using NUnit.Framework;

namespace NHibernate.Test.Linq.ByMethod
{
	[TestFixture]
	public class GroupByTests : LinqTestCase
	{
		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Cfg.Environment.ShowSql, "true");
		}

		[Test]
		public void SingleKeyGroupAndCount()
		{
			var orderCounts = db.Orders.GroupBy(o => o.Customer).Select(g => g.Count()).ToList();
			Assert.AreEqual(89, orderCounts.Count);
			Assert.AreEqual(830, orderCounts.Sum());
		}

		[Test]
		public void MultipleKeyGroupAndCount()
		{
			var orderCounts = db.Orders.GroupBy(o => new { o.Customer, o.Employee }).Select(g => g.Count()).ToList();
			Assert.AreEqual(464, orderCounts.Count);
			Assert.AreEqual(830, orderCounts.Sum());
		}

		[Test]
		public void SingleKeyGrouping()
		{
			var orders = db.Orders.GroupBy(o => o.Customer).ToList();
			Assert.That(orders.Count, Is.EqualTo(89));
			Assert.That(orders.Sum(o => o.Count()), Is.EqualTo(830));
			CheckGrouping(orders, o => o.Customer);
		}

		[Test]
		public void MultipleKeyGrouping()
		{
			var orders = db.Orders.GroupBy(o => new { o.Customer, o.Employee }).ToList();
			Assert.That(orders.Count, Is.EqualTo(464));
			Assert.That(orders.Sum(o => o.Count()), Is.EqualTo(830));

			CheckGrouping(
				orders.Select(g => new TupGrouping<Customer, Employee, Order>(g.Key.Customer, g.Key.Employee, g)),
				o => o.Customer,
				o => o.Employee);
		}

		[Test]
		public void GroupBySelectKeyShouldUseServerSideGrouping()
		{
			using (var spy = new SqlLogSpy())
			{
				var orders = (from o in db.Orders
							  group o by o.OrderDate
								  into g
								  select g.Key).ToList();

				Assert.That(orders.Count, Is.EqualTo(481));
				Assert.That(Regex.Replace(spy.GetWholeLog(), @"\s+", " "), Is.StringContaining("group by order0_.OrderDate"));
			}
		}

		[Test]
		public void SingleKeyGroupAndOrderByKey()
		{
			//NH-2452
			var result = db.Products
				.GroupBy(i => i.UnitPrice)
				.OrderBy(g => g.Key)
				.Select(g => new
								 {
									 UnitPrice = g.Max(i => i.UnitPrice),
									 TotalUnitsInStock = g.Sum(i => i.UnitsInStock)
								 })
				.ToList();

			Assert.That(result.Count, Is.EqualTo(62));
			AssertOrderedBy.Ascending(result, x => x.UnitPrice);
		}

		[Test]
		public void SingleKeyGroupAndOrderByKeyAggregateProjection()
		{
			//NH-2452
			var result = db.Products
				.GroupBy(i => i.UnitPrice)
				.Select(g => new
								 {
									 UnitPrice = g.Max(i => i.UnitPrice),
									 TotalUnitsInStock = g.Sum(i => i.UnitsInStock)
								 })
				.OrderBy(x => x.UnitPrice)
				.ToList();

			Assert.That(result.Count, Is.EqualTo(62));
			AssertOrderedBy.Ascending(result, x => x.UnitPrice);
		}

		[Test]
		public void SingleKeyPropertyGroupAndOrderByProjectedCount()
		{
			// NH-2560

			var orderCounts = db.Orders
				.GroupBy(o => o.Customer.CustomerId)
				.Select(g =>
				new
				{
					CustomerId = g.Key,
					OrderCount = g.Count()
				})
				.OrderByDescending(t => t.OrderCount)
				.ToList();

			AssertOrderedBy.Descending(orderCounts, oc => oc.OrderCount);
		}


		[Test]
		public void SingleKeyPropertyGroupAndOrderByCountBeforeProjection()
		{
			// NH-3026, variation of NH-2560.
			// This is a variation of SingleKeyPropertyGroupAndOrderByProjectedCount()
			// that puts the ordering expression inside the OrderBy, without first
			// going through a select clause.

			var orderCounts = db.Orders
				.GroupBy(o => o.Customer.CustomerId)
				.OrderByDescending(g => g.Count())
				.Select(g =>
				new
				{
					CustomerId = g.Key,
					OrderCount = g.Count()
				})
				.ToList();

			AssertOrderedBy.Descending(orderCounts, oc => oc.OrderCount);
		}


		[Test]
		[Ignore("Generates incorrect SQL. Reported as NH-3027.")]
		public void SingleKeyPropertyGroupByEntityAndSelectEntity()
		{
			// NH-3027

			var orderCounts = db.Orders
				.GroupBy(o => o.Customer)
				.Select(g => new { Customer = g.Key, OrderCount = g.Count() })
				.OrderByDescending(t => t.OrderCount)
				.ToList();

			AssertOrderedBy.Descending(orderCounts, oc => oc.OrderCount);
		}


		[Test]
		public void SingleKeyGroupAndOrderByNonKeyAggregateProjection()
		{
			//NH-2452
			var result = db.Products
				.GroupBy(p => p.UnitPrice)
				.Select(g => new
								 {
									 UnitPrice = g.Max(i => i.UnitPrice),
									 TotalUnitsInStock = g.Sum(i => i.UnitsInStock)
								 })
				.OrderBy(x => x.TotalUnitsInStock)
				.ToList();

			Assert.That(result.Count, Is.EqualTo(62));
			AssertOrderedBy.Ascending(result, x => x.TotalUnitsInStock);
		}

		[Test]
		public void SingleKeyGroupAndOrderByKeyProjection()
		{
			//NH-2452
			var result = db.Products
				.GroupBy(i => i.UnitPrice)
				.Select(g => new
								 {
									 UnitPrice = g.Key,
									 TotalUnitsInStock = g.Sum(i => i.UnitsInStock)
								 })
				.OrderBy(x => x.UnitPrice)
				.ToList();

			Assert.That(result.Count, Is.EqualTo(62));
			AssertOrderedBy.Ascending(result, x => x.UnitPrice);
		}

		[Test]
		public void GroupByWithAndAlsoContainsInWhereClause()
		{
			//NH-3032
			var collection = db.Products.Select(x => x.Supplier).ToList();

			var result = db.Products
				.Where(x => x.Discontinued == true && collection.Contains(x.Supplier))
				.GroupBy(x => x.UnitPrice)
				.Select(x => new {x.Key, Count = x.Count()})
				.ToList();
 
			Assert.That(result.Count, Is.EqualTo(8));
		}

		[Test]
		public void GroupByWithContainsInWhereClause()
		{
			//NH-3032
			var collection = db.Products.Select(x => x.Supplier).ToList();

			var result = db.Products
				.Where(x => collection.Contains(x.Supplier))
				.GroupBy(x => x.UnitPrice)
				.Select(x => new {x.Key, Count = x.Count()})
				.ToList();
 
			Assert.That(result.Count, Is.EqualTo(62));
		}

		private static void CheckGrouping<TKey, TElement>(IEnumerable<IGrouping<TKey, TElement>> groupedItems, Func<TElement, TKey> groupBy)
		{
			var used = new HashSet<object>();
			foreach (IGrouping<TKey, TElement> group in groupedItems)
			{
				Assert.IsFalse(used.Contains(group.Key));
				used.Add(group.Key);

				foreach (var item in group)
				{
					Assert.AreEqual(group.Key, groupBy(item));
				}
			}
		}

		private static void CheckGrouping<TKey1, TKey2, TElement>(IEnumerable<TupGrouping<TKey1, TKey2, TElement>> groupedItems, Func<TElement, TKey1> groupBy1, Func<TElement, TKey2> groupBy2)
		{
			var used = new HashSet<object>();
			foreach (IGrouping<Tup<TKey1, TKey2>, TElement> group in groupedItems)
			{
				Assert.IsFalse(used.Contains(group.Key));
				used.Add(group.Key);

				foreach (var item in group)
				{
					Assert.AreEqual(group.Key.Item1, groupBy1(item));
					Assert.AreEqual(group.Key.Item2, groupBy2(item));
				}
			}
		}

		private class TupGrouping<TKey1, TKey2, TElement> : IGrouping<Tup<TKey1, TKey2>, TElement>
		{
			private IEnumerable<TElement> Elements { get; set; }

			public TupGrouping(TKey1 key1, TKey2 key2, IEnumerable<TElement> elements)
			{
				Key = new Tup<TKey1, TKey2>(key1, key2);
				Elements = elements;
			}

			public IEnumerator<TElement> GetEnumerator()
			{
				return Elements.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public Tup<TKey1, TKey2> Key { get; private set; }
		}

		private class Tup<T1, T2>
		{
			public T1 Item1 { get; private set; }
			public T2 Item2 { get; private set; }

			public Tup(T1 item1, T2 item2)
			{
				Item1 = item1;
				Item2 = item2;
			}

			public override bool Equals(object obj)
			{
				if (obj == null)
					return false;

				if (obj.GetType() != GetType())
					return false;

				var other = (Tup<T1, T2>)obj;

				return Equals(Item1, other.Item1) && Equals(Item2, other.Item2);
			}

			public override int GetHashCode()
			{
				return Item1.GetHashCode() ^ Item2.GetHashCode();
			}
		}
	}
}

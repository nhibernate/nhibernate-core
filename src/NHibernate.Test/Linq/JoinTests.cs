using System;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class JoinTests : LinqTestCase
	{
		[Test]
		public void OrderLinesWith2ImpliedJoinShouldProduce2JoinsInSql()
		{
			//NH-3003
			using (var spy = new SqlLogSpy())
			{
				var lines = (from l in db.OrderLines
							 where l.Order.Customer.CompanyName == "Vins et alcools Chevalier"
							 select l).ToList();

				Assert.AreEqual(10, lines.Count);
				var countJoins = CountJoins(spy);
				Assert.That(countJoins, Is.EqualTo(2));
			}
		}

		[Test]
		public void OrderLinesWith2ImpliedJoinByIdShouldNotContainImpliedJoin()
		{
			//NH-2946 + NH-3003 = NH-2451
			using (var spy = new SqlLogSpy())
			{
				var lines = (from l in db.OrderLines
							 where l.Order.Customer.CustomerId == "VINET"
							 where l.Order.Customer.CompanyName == "Vins et alcools Chevalier"
							 select l).ToList();

				Assert.AreEqual(10, lines.Count);
				var countJoins = CountJoins(spy);
				Assert.That(countJoins, Is.EqualTo(2));
				Assert.That(Count(spy, "Orders"), Is.EqualTo(1));
			}
		}
		
		[Test]
		public void OrderLinesFilterByCustomerIdSelectLineShouldNotContainJoinWithCustomer()
		{
			//NH-2946
			using (var spy = new SqlLogSpy())
			{
				var lines = (from l in db.OrderLines
							 where l.Order.Customer.CustomerId == "VINET"
							 select l).ToList();

				Assert.AreEqual(10, lines.Count);
				var countJoins = CountJoins(spy);
				Assert.That(countJoins, Is.EqualTo(1));
				Assert.That(Count(spy, "Customers"), Is.EqualTo(0));
			}
		}
		
		[Test]
		public void OrderLinesFilterByCustomerIdSelectCustomerIdShouldNotContainJoinWithCustomer()
		{
			//NH-2946
			using (var spy = new SqlLogSpy())
			{
				var lines = (from l in db.OrderLines
							 where l.Order.Customer.CustomerId == "VINET"
							 select l.Order.Customer.CustomerId).ToList();

				Assert.AreEqual(10, lines.Count);
				var countJoins = CountJoins(spy);
				Assert.That(countJoins, Is.EqualTo(1));
				Assert.That(Count(spy, "Customers"), Is.EqualTo(0));
			}
		}
		
		[Test]
		public void OrderLinesFilterByCustomerIdSelectCustomerShouldContainJoinWithCustomer()
		{
			//NH-2946
			using (var spy = new SqlLogSpy())
			{
				var lines = (from l in db.OrderLines
							 where l.Order.Customer.CustomerId == "VINET"
							 select l.Order.Customer).ToList();

				Assert.AreEqual(10, lines.Count);
				var countJoins = CountJoins(spy);
				Assert.That(countJoins, Is.EqualTo(2));
				Assert.That(Count(spy, "Customers"), Is.EqualTo(1));
			}
		}
		
		[Test]
		public void OrderLinesFilterByCustomerCompanyNameAndSelectCustomerIdShouldJoinOrdersOnlyOnce()
		{
			//NH-2946 + NH-3003 = NH-2451
			using (var spy = new SqlLogSpy())
			{
				var lines = (from l in db.OrderLines
							 where l.Order.Customer.CompanyName == "Vins et alcools Chevalier"
							 select l.Order.Customer.CustomerId).ToList();

				Assert.AreEqual(10, lines.Count);
				var countJoins = CountJoins(spy);
				Assert.That(countJoins, Is.EqualTo(2));
				Assert.That(Count(spy, "Orders"), Is.EqualTo(1));
			}
		}
		
		[Test]
		public void OrderLinesFilterByOrderDateAndSelectOrderId()
		{
			//NH-2451
			using (var spy = new SqlLogSpy())
			{
				var lines = (from l in db.OrderLines
							 where l.Order.OrderDate < DateTime.Now
							 select l.Order.OrderId).ToList();

				Assert.AreEqual(2155, lines.Count);
				var countJoins = CountJoins(spy);
				Assert.That(countJoins, Is.EqualTo(1));
			}
		}

		[Test]
		public void OrderLinesFilterByOrderIdAndSelectOrderDate()
		{
			//NH-2451
			using (var spy = new SqlLogSpy())
			{
				var lines = (from l in db.OrderLines
							 where l.Order.OrderId == 100
							 select l.Order.OrderDate).ToList();

				var countJoins = CountJoins(spy);
				Assert.That(countJoins, Is.EqualTo(1));
				Assert.That(Count(spy, "Orders"), Is.EqualTo(1));
			}
		}

		[Test]
		public void OrderLinesFilterByOrderIdAndSelectOrder()
		{
			//NH-2946
			using (var spy = new SqlLogSpy())
			{
				var lines = (from l in db.OrderLines
							 where l.Order.OrderId == 100
							 select l.Order).ToList();

				var countJoins = CountJoins(spy);
				Assert.That(countJoins, Is.EqualTo(1));
				Assert.That(Count(spy, "Orders"), Is.EqualTo(1));
			}
		}

		[Test]
		public void OrderLinesWithFilterByOrderIdShouldNotProduceJoins()
		{
			//NH-2946
			using (var spy = new SqlLogSpy())
			{
				(from l in db.OrderLines
				 where l.Order.OrderId == 1000
				 select l).ToList();

				var countJoins = CountJoins(spy);
				Assert.That(countJoins, Is.EqualTo(0));
			}
		}
		
		[Test]
		public void OrderLinesWithFilterByOrderIdAndDateShouldProduceOneJoin()
		{
			//NH-2946
			using (var spy = new SqlLogSpy())
			{
				(from l in db.OrderLines
				 where l.Order.OrderId == 1000 && l.Order.OrderDate < DateTime.Now
				 select l).ToList();

				var countJoins = CountJoins(spy);
				Assert.That(countJoins, Is.EqualTo(1));
			}
		}

		[Test]
		public void OrderLinesWithOrderByOrderIdShouldNotProduceJoins()
		{
			//NH-2946
			using (var spy = new SqlLogSpy())
			{
				(from l in db.OrderLines
				 orderby l.Order.OrderId
				 select l).ToList();

				var countJoins = CountJoins(spy);
				Assert.That(countJoins, Is.EqualTo(0));
			}
		}

		[Test]
		public void OrderLinesWithOrderByOrderShouldNotProduceJoins()
		{
			//NH-2946
			using (var spy = new SqlLogSpy())
			{
				(from l in db.OrderLines
				 orderby l.Order
				 select l).ToList();

				var countJoins = CountJoins(spy);
				Assert.That(countJoins, Is.EqualTo(0));
			}
		}

		[Test]
		public void OrderLinesWithOrderByOrderIdAndDateShouldProduceOneJoin()
		{
			//NH-2946
			using (var spy = new SqlLogSpy())
			{
				(from l in db.OrderLines
				 orderby l.Order.OrderId, l.Order.OrderDate
				 select l).ToList();

				var countJoins = CountJoins(spy);
				Assert.That(countJoins, Is.EqualTo(1));
			}
		}

		[Test]
		public void OrderLinesWithSelectingOrderIdShouldNotProduceJoins()
		{
			//NH-2946
			using (var spy = new SqlLogSpy())
			{
				(from l in db.OrderLines
				 select l.Order.OrderId).ToList();

				var countJoins = CountJoins(spy);
				Assert.That(countJoins, Is.EqualTo(0));
			}
		}

		[Test]
		public void OrderLinesWithSelectingOrderIdAndDateShouldProduceOneJoin()
		{
			//NH-2946
			using (var spy = new SqlLogSpy())
			{
				(from l in db.OrderLines
				 select new {l.Order.OrderId, l.Order.OrderDate}).ToList();

				var countJoins = CountJoins(spy);
				Assert.That(countJoins, Is.EqualTo(1));
			}
		}

		private static int CountJoins(LogSpy sqlLog)
		{
			return Count(sqlLog, "join");
		}

		private static int Count(LogSpy sqlLog, string s)
		{
			var log = sqlLog.GetWholeLog();
			return log.Split(new[] {s}, StringSplitOptions.None).Length - 1;
		}
	}
}

using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.DomainModel.Northwind.Entities;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class MathTests : LinqTestCase
	{
		private IQueryable<OrderLine> _orderLines;

		protected override void OnSetUp()
		{
			base.OnSetUp();
			_orderLines = db.OrderLines
							.OrderBy(ol => ol.Id)
							.Take(10).ToList().AsQueryable();
		}

		[Test]
		public void SignAllPositiveTest()
		{
			AssumeFunctionSupported("sign");
			var signs = (from o in db.OrderLines
						 select Math.Sign(o.UnitPrice)).ToList();

			Assert.That(signs.All(x => x == 1), Is.True);
		}

		[Test]
		public void SignAllNegativeTest()
		{
			AssumeFunctionSupported("sign");
			var signs = (from o in db.OrderLines
						 select Math.Sign(0m - o.UnitPrice)).ToList();

			Assert.That(signs.All(x => x == -1), Is.True);
		}

		[Test]
		public void SinTest()
		{
			AssumeFunctionSupported("sin");
			Test(o => Math.Round(Math.Sin((double) o.UnitPrice), 5));
		}

		[Test]
		public void CosTest()
		{
			AssumeFunctionSupported("cos");
			Test(o => Math.Round(Math.Cos((double)o.UnitPrice), 5));
		}

		[Test]
		public void TanTest()
		{
			AssumeFunctionSupported("tan");
			Test(o => Math.Round(Math.Tan((double)o.Discount), 5));
		}

		[Test]
		public void SinhTest()
		{
			AssumeFunctionSupported("sinh");
			Test(o => Math.Round(Math.Sinh((double)o.Discount), 5));
		}

		[Test]
		public void CoshTest()
		{
			AssumeFunctionSupported("cosh");
			Test(o => Math.Round(Math.Cosh((double)o.Discount), 5));
		}

		[Test]
		public void TanhTest()
		{
			AssumeFunctionSupported("tanh");
			Test(o => Math.Round(Math.Tanh((double)o.Discount), 5));
		}

		[Test]
		public void AsinTest()
		{
			AssumeFunctionSupported("asin");
			Test(o => Math.Round(Math.Asin((double)o.Discount), 5));
		}

		[Test]
		public void AcosTest()
		{
			AssumeFunctionSupported("acos");
			Test(o => Math.Round(Math.Acos((double)o.Discount), 5));
		}

		[Test]
		public void AtanTest()
		{
			AssumeFunctionSupported("atan");
			Test(o => Math.Round(Math.Atan((double)o.UnitPrice), 5));
		}

		[Test]
		public void Atan2Test()
		{
			AssumeFunctionSupported("atan2");
			Test(o => Math.Round(Math.Atan2((double)o.Discount, 0.5d), 5));
		}

		[Test]
		public void PowTest()
		{
			AssumeFunctionSupported("power");
			Test(o => Math.Round(Math.Pow((double)o.Discount, 0.5d), 5));
		}

		[Test]
		public void MinTest()
		{
			AssumeFunctionSupported("least");
			Test(o => Math.Min((double) o.UnitPrice, 20d));
		}

		[Test]
		public void MaxTest()
		{
			AssumeFunctionSupported("greatest");
			Test(o => Math.Max((double) o.UnitPrice, 20d));
		}

		[Test]
		public void MinDecimalTest()
		{
			AssumeFunctionSupported("least");
			TestExact(o => Math.Min(o.UnitPrice, 20m));
		}

		[Test]
		public void MaxDecimalTest()
		{
			AssumeFunctionSupported("greatest");
			TestExact(o => Math.Max(o.UnitPrice, 20m));
		}

		[Test]
		public void MinInt32Test()
		{
			AssumeFunctionSupported("least");
			TestExact(o => Math.Min(o.Quantity, 20));
		}

		[Test]
		public void MaxInt32Test()
		{
			AssumeFunctionSupported("greatest");
			TestExact(o => Math.Max(o.Quantity, 20));
		}

		[Test]
		public void MinOnTwoColumnsTest()
		{
			AssumeFunctionSupported("least");
			TestExact(o => Math.Min(o.UnitPrice, o.Discount));
		}

		[Test]
		public void MaxOnTwoColumnsTest()
		{
			AssumeFunctionSupported("greatest");
			TestExact(o => Math.Max(o.UnitPrice, o.Discount));
		}

		[Test]
		public void NestedMinTest()
		{
			AssumeFunctionSupported("least");
			TestExact(o => Math.Min(Math.Min(o.UnitPrice, o.Discount), 20m));
		}

		private void Test(Expression<Func<OrderLine, double>> selector)
		{
			var expected = _orderLines
				.Select(selector)
				.ToList();

			var actual = db.OrderLines
				.OrderBy(ol => ol.Id)
				.Select(selector)
				.Take(10)
				.ToList();

			Assert.AreEqual(expected.Count, actual.Count);
			for (var i = 0; i < expected.Count; i++)
				Assert.AreEqual(expected[i], actual[i], 0.000001);
		}

		private void TestExact<T>(Expression<Func<OrderLine, T>> selector)
		{
			var expected = _orderLines
				.Select(selector)
				.ToList();

			var actual = db.OrderLines
				.OrderBy(ol => ol.Id)
				.Select(selector)
				.Take(10)
				.ToList();

			Assert.That(actual, Is.EqualTo(expected));
		}
	}
}

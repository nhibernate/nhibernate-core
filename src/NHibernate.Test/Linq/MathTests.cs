using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Dialect;
using NHibernate.DomainModel.Northwind.Entities;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class MathTests : LinqTestCase
	{
		private IQueryable<OrderLine> _orderLines;

		private void IgnoreIfNotSupported(string function)
		{
			if (!Dialect.Functions.ContainsKey(function))
				Assert.Ignore("Dialect {0} does not support '{1}' function", Dialect.GetType(), function);
		}

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
			IgnoreIfNotSupported("sign");
			var signs = (from o in db.OrderLines
						 select Math.Sign(o.UnitPrice)).ToList();

			Assert.True(signs.All(x => x == 1));
		}

		[Test]
		public void SignAllNegativeTest()
		{
			IgnoreIfNotSupported("sign");
			var signs = (from o in db.OrderLines
						 select Math.Sign(0m - o.UnitPrice)).ToList();

			Assert.True(signs.All(x => x == -1));
		}

		[Test]
		public void SinTest()
		{
			IgnoreIfNotSupported("sin");
			Test(o => Math.Round(Math.Sin((double) o.UnitPrice), 5));
		}

		[Test]
		public void CosTest()
		{
			IgnoreIfNotSupported("cos");
			Test(o => Math.Round(Math.Cos((double)o.UnitPrice), 5));
		}

		[Test]
		public void TanTest()
		{
			IgnoreIfNotSupported("tan");
			Test(o => Math.Round(Math.Tan((double)o.Discount), 5));
		}

		[Test]
		public void SinhTest()
		{
			IgnoreIfNotSupported("sinh");
			Test(o => Math.Round(Math.Sinh((double)o.Discount), 5));
		}

		[Test]
		public void CoshTest()
		{
			IgnoreIfNotSupported("cosh");
			Test(o => Math.Round(Math.Cosh((double)o.Discount), 5));
		}

		[Test]
		public void TanhTest()
		{
			IgnoreIfNotSupported("tanh");
			Test(o => Math.Round(Math.Tanh((double)o.Discount), 5));
		}

		[Test]
		public void AsinTest()
		{
			IgnoreIfNotSupported("asin");
			Test(o => Math.Round(Math.Asin((double)o.Discount), 5));
		}

		[Test]
		public void AcosTest()
		{
			IgnoreIfNotSupported("acos");
			Test(o => Math.Round(Math.Acos((double)o.Discount), 5));
		}

		[Test]
		public void AtanTest()
		{
			IgnoreIfNotSupported("atan");
			Test(o => Math.Round(Math.Atan((double)o.UnitPrice), 5));
		}

		[Test]
		public void Atan2Test()
		{
			IgnoreIfNotSupported("atan2");
			Test(o => Math.Round(Math.Atan2((double)o.Discount, 0.5d), 5));
		}

		[Test]
		public void PowTest()
		{
			IgnoreIfNotSupported("power");
			Test(o => Math.Round(Math.Pow((double)o.Discount, 0.5d), 5));
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
	}
}

#if NETCOREAPP2_0
using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.DomainModel.Northwind.Entities;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	public class MathFTests : LinqTestCase
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
			             select MathF.Sign((float) o.UnitPrice)).ToList();

			Assert.That(signs.All(x => x == 1), Is.True);
		}

		[Test]
		public void SignAllNegativeTest()
		{
			AssumeFunctionSupported("sign");
			var signs = (from o in db.OrderLines
			             select MathF.Sign(0f - (float) o.UnitPrice)).ToList();

			Assert.That(signs.All(x => x == -1), Is.True);
		}

		[Test]
		public void SinTest()
		{
			AssumeFunctionSupported("sin");
			Test(o => MathF.Round(MathF.Sin((float) o.UnitPrice), 5));
		}

		[Test]
		public void CosTest()
		{
			AssumeFunctionSupported("cos");
			Test(o => MathF.Round(MathF.Cos((float)o.UnitPrice), 5));
		}

		[Test]
		public void TanTest()
		{
			AssumeFunctionSupported("tan");
			Test(o => MathF.Round(MathF.Tan((float)o.Discount), 5));
		}

		[Test]
		public void SinhTest()
		{
			AssumeFunctionSupported("sinh");
			Test(o => MathF.Round(MathF.Sinh((float)o.Discount), 5));
		}

		[Test]
		public void CoshTest()
		{
			AssumeFunctionSupported("cosh");
			Test(o => MathF.Round(MathF.Cosh((float)o.Discount), 5));
		}

		[Test]
		public void TanhTest()
		{
			AssumeFunctionSupported("tanh");
			Test(o => MathF.Round(MathF.Tanh((float)o.Discount), 5));
		}

		[Test]
		public void AsinTest()
		{
			AssumeFunctionSupported("asin");
			Test(o => MathF.Round(MathF.Asin((float)o.Discount), 5));
		}

		[Test]
		public void AcosTest()
		{
			AssumeFunctionSupported("acos");
			Test(o => MathF.Round(MathF.Acos((float)o.Discount), 5));
		}

		[Test]
		public void AtanTest()
		{
			AssumeFunctionSupported("atan");
			Test(o => MathF.Round(MathF.Atan((float)o.UnitPrice), 5));
		}

		[Test]
		public void Atan2Test()
		{
			AssumeFunctionSupported("atan2");
			Test(o => MathF.Round(MathF.Atan2((float)o.Discount, 0.5f), 5));
		}

		[Test]
		public void PowTest()
		{
			AssumeFunctionSupported("power");
			Test(o => MathF.Round(MathF.Pow((float)o.Discount, 0.5f), 5));
		}

		private void Test(Expression<Func<OrderLine, float>> selector)
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
#endif

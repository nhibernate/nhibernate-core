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
			_orderLines = db.OrderLines.Take(10).ToList().AsQueryable();
		}

		[Test]
		public void SignAllPositiveTest()
		{
			var signs = (from o in db.OrderLines
						 select Math.Sign(o.UnitPrice)).ToList();

			Assert.True(signs.All(x => x == 1));
		}

		[Test]
		public void SignAllNegativeTest()
		{
			var signs = (from o in db.OrderLines
						 select Math.Sign(-1*o.UnitPrice)).ToList();

			Assert.True(signs.All(x => x == -1));
		}

		[Test]
		public void SinTest()
		{
			Test(o => Math.Sin((double) o.UnitPrice));
		}

		[Test]
		public void CosTest()
		{
			Test(o => Math.Cos((double) o.UnitPrice));
		}
		
		[Test]
		public void TanTest()
		{
			Test(o => Math.Tan((double) o.UnitPrice));
		}

		[Test]
		public void SinhTest()
		{
			Test(o => Math.Sinh((double)o.UnitPrice));
		}

		[Test]
		public void CoshTest()
		{
			Test(o => Math.Cosh((double)o.UnitPrice));
		}

		[Test]
		public void TanhTest()
		{
			Test(o => Math.Tanh((double)o.UnitPrice));
		}

		[Test]
		public void AsinTest()
		{
			Test(o => Math.Asin((double) o.UnitPrice));
		}

		[Test]
		public void AcosTest()
		{
			Test(o => Math.Acos((double) o.UnitPrice));
		}
		
		[Test]
		public void AtanTest()
		{
			Test(o => Math.Atan((double) o.UnitPrice));
		}

		[Test]
		public void Atan2Test()
		{
			Test(o => Math.Atan2((double) o.UnitPrice, 0.5d));
		}

		private void Test<T>(Expression<Func<OrderLine, T>> selector)
		{
			var expected = _orderLines
				.Select(selector)
				.ToList();

			var actual = db.OrderLines.Select(selector)
				.Take(10)
				.ToList();

			Assert.AreEqual(actual, expected);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq.ByMethod
{
	[TestFixture]
	public class MappedAsTests : LinqTestCase
	{
		[Test]
		public void WithUnaryExpression()
		{
			var num = 1;
			db.Orders.Where(o => o.Freight == (-num).MappedAs(NHibernateUtil.Decimal)).ToList();
			db.Orders.Where(o => o.Freight == ((decimal) num).MappedAs(NHibernateUtil.Decimal)).ToList();
			db.Orders.Where(o => o.Freight == ((decimal?) (decimal) num).MappedAs(NHibernateUtil.Decimal)).ToList();
		}

		[Test]
		public void WithNewExpression()
		{
			var num = 1;
			db.Orders.Where(o => o.Freight == new decimal(num).MappedAs(NHibernateUtil.Decimal)).ToList();
		}

		[Test]
		public void WithMethodCallExpression()
		{
			var num = 1;
			db.Orders.Where(o => o.Freight == GetDecimal(num).MappedAs(NHibernateUtil.Decimal)).ToList();
		}

		private decimal GetDecimal(int number)
		{
			return number;
		}
	}
}

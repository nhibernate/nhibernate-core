using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
    [TestFixture]
    public class OperatorTests : LinqTestCase
    {
        [Test]
        public void Mod()
        {
            Assert.AreEqual(2, session.Query<TimesheetEntry>().Where(a => a.NumberOfHours % 7 == 0).Count());
        }

		[Test]
		public void UnaryMinus()
		{
			Assert.AreEqual(1, session.Query<TimesheetEntry>().Count(a => -a.NumberOfHours == -7));
		}

		[Test]
		public void DecimalAdd()
		{
			decimal offset = 5.5m;
			decimal test = 10248 + offset;
			var result = session.Query<Order>().Where(e => offset + e.OrderId == test).ToList();
			Assert.That(result, Has.Count.EqualTo(1));

			offset = 5.5m;
			test = 32.38m + offset;
			result = session.Query<Order>().Where(e => offset + e.Freight == test).ToList();
			Assert.That(result, Has.Count.EqualTo(1));
		}

		[Test]
		public void UnaryPlus()
		{
			// Ensure expression tree contains UnaryPlus
			var param = Expression.Parameter(typeof(TimesheetEntry), "e");
			var expr = Expression.Equal(Expression.UnaryPlus(Expression.PropertyOrField(param, "NumberOfHours")), Expression.Constant(7));
			var predicate = Expression.Lambda<Func<TimesheetEntry, bool>>(expr, param);
			Assert.AreEqual(1, session.Query<TimesheetEntry>().Count(predicate));
		}
	}
}

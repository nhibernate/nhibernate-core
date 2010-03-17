using System;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
    [TestFixture]
    public class DateTimeTests : LinqTestCase
    {
        [Test]
        public void CanQueryByYear()
        {
            var x = (from o in db.Orders
                     where o.OrderDate.Value.Year == 1998
                     select o).ToList();

            Assert.AreEqual(270, x.Count());
        }

        [Test]
        public void CanQueryByDate()
        {
            var x = (from o in db.Orders
                    where o.OrderDate.Value.Date == new DateTime(1998, 02, 26)
                    select o).ToList();

            Assert.AreEqual(6, x.Count());
        }

        [Test]
        public void CanQueryByDateTime()
        {
            var x = (from o in db.Orders
                     where o.OrderDate.Value == new DateTime(1998, 02, 26)
                     select o).ToList();

            Assert.AreEqual(5, x.Count());
        }

        [Test]
        public void CanQueryByDateTime2()
        {
            var x = (from o in db.Orders
                     where o.OrderDate.Value == new DateTime(1998, 02, 26, 0, 1, 0)
                     select o).ToList();

            Assert.AreEqual(1, x.Count());
        }
    }
}

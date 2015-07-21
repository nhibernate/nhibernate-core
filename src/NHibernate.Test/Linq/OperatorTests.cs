using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}

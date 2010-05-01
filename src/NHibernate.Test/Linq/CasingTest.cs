using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
    [TestFixture]
    public class CasingTest : LinqTestCase
    {
        [Test]
        public void ToUpper()
        {
            var name = (from e in db.Employees
                        where e.EmployeeId == 1
                        select e.FirstName.ToUpper())
                       .Single();
            Assert.AreEqual("NANCY", name);
        }

        [Test]
        public void ToUpperInvariant()
        {
            var name = (from e in db.Employees
                        where e.EmployeeId == 1
                        select e.FirstName.ToUpper())
                       .Single();
            Assert.AreEqual("NANCY", name);
        }

        [Test]
        public void ToLower()
        {
            var name = (from e in db.Employees
                        where e.EmployeeId == 1
                        select e.FirstName.ToLower())
                       .Single();
            Assert.AreEqual("nancy", name);
        }

        [Test]
        public void ToLowerInvariant()
        {
            var name = (from e in db.Employees
                        where e.EmployeeId == 1
                        select e.FirstName.ToLowerInvariant())
                       .Single();
            Assert.AreEqual("nancy", name);
        }
    }
}
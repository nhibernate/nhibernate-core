using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
    [TestFixture]
    public class BasicLinqTests : LinqTestCase
    {
        [Test]
        public void DummySelect()
        {
            var soldOutProducts = from p in db.Products select p;

            var results = soldOutProducts.ToList();

            Assert.AreEqual(0, results.Count);
        }
    }
}
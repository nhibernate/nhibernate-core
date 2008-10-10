using NUnit.Framework;

namespace NHibernate.PostSharp.Proxies.Tests
{
    [TestFixture]
    public class SavingInstanceToDatabase : TestCase
    {
        [Test]
        public void CanSaveItemToDatabase()
        {
            int custId;
            using (var s = sessions.OpenSession())
            using (var tx = s.BeginTransaction())
            {
                var obj = new Customer { Name = "ayende" };
                s.Save(obj);
                tx.Commit();
                Assert.AreNotEqual(0, obj.Id);
                custId = obj.Id;
            }
            using (var s = sessions.OpenSession())
            using (var tx = s.BeginTransaction())
            {
                var load = s.Get<Customer>(custId);
                s.Delete(load);
                tx.Commit();
            }
        }

    }
}
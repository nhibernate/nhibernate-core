using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1025
{
    [TestFixture]
    public class Fixture : BugTestCase
    {
        [Test]
        public void Test()
        {
            var e = new Entity();
            e.Components.Add(new Component(1, 2, null));
            e.Components.Add(new Component(null, 2, 3));
            e.Components.Add(new Component(1, null, 3));

            // Save our entity with 3 Components that contain null-valued properties
            // null values are required to generate invalid SQL.
            using (ISession s = OpenSession())
            using (ITransaction t = s.BeginTransaction())
            {
                s.SaveOrUpdate(e);
                t.Commit();
            }

            // Retrieve entity, and add an additional component. Incorrect SQL will include
            // 'WHERE ValueA = NULL AND ...' etc. Correct SQL should be 'WHERE ValueA IS NULL AND ...' etc.
            // This causes the old component records to not be found, and the new collection gets appended
            // to the old collection, resulting in OriginalCollection.Count + ModifiedCollection.Count records.
            using (ISession s = OpenSession())
            using (ITransaction t = s.BeginTransaction())
            {
                var e2 = s.Get<Entity>(e.Id);
                e2.Components.Add(new Component(null, null, 3));

                s.SaveOrUpdate(e2);
                t.Commit();
            }

            // NH1025 results in 7 retrieved Components, instead of the expected 4.
            using (ISession s = OpenSession())
            using (ITransaction t = s.BeginTransaction())
            {
                var e3 =s.Get<Entity>(e.Id);
                Assert.AreEqual(4, e3.Components.Count);
            }
        }

        protected override void OnTearDown()
        {
            using (var s = OpenSession())
            using (var t = s.BeginTransaction())
            {
                s.Delete("from Entity");
                t.Commit();
            }
        }
    }
}

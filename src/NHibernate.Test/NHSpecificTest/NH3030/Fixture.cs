using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3030
{
	[TestFixture]
	public class Fixture : BugTestCase
	{

		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (var session = OpenSession())
			{
				session.Save(new EntityWithoutDefaultCtor("fully persistence ignorant", new ComponentWithoutDefaultCtor("USD", 100)), 1);
                session.Save(new EntityWithoutDefaultCtor("completely persistence ignorant"), 2);
                session.Save(new EntityWithoutDefaultCtor("totally persistence ignorant"), 3);
				session.Flush();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (var session = OpenSession())
			{
				session.Delete("from System.Object");
				session.Flush();
			}
		}

		[Test]
		public void EntityWithoutDefaultCtorShouldBeSuccessfullyMaterialized()
		{
			using (var session = OpenSession())
			{
			    var entity3 = session.QueryOver<EntityWithoutDefaultCtor>().Where(e => e.Name == "totally persistence ignorant").SingleOrDefault();
                Assert.That(entity3, Is.Not.Null);
                Assert.That(entity3.Name, Is.EqualTo("totally persistence ignorant"));
			    var entity1 = session.Get<EntityWithoutDefaultCtor>(1);
                Assert.That(entity1, Is.Not.Null);
                Assert.That(entity1.Name, Is.EqualTo("fully persistence ignorant"));
            }
		}


        [Test]
        public void ComponentWithoutDefaultCtorShouldBeSuccessfullyMaterialized()
        {
            using (var session = OpenSession())
            {
                var entity = session.Get<EntityWithoutDefaultCtor>(1);
                Assert.That(entity.Money, Is.Not.Null);
                Assert.AreEqual(100m, entity.Money.Amount);
                Assert.AreEqual("USD", entity.Money.Currency);
            }
        }
	}
}
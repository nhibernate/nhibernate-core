using System.Collections.Generic;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3694
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			//noop
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

        [Test]
        public void ListEntitiesWithComponentsCollectionPropertyAndRestricionAppliedToThem()
        {
            const string val1 = "one";
            var entity = new EntityWithComponentsCollection
            {
                Components = new HashSet<Component>
                        {
                            new Component()
                                {
                                    Val1 = val1,
                                    Val2 = "two",
                                },
                            new Component()
                                {
                                    Val1 = "I",
                                    Val2 = "II"
                                }
                        }
            };
            using (ISession session = this.OpenSession())
            {
                session.Save(entity);
                session.Flush();
            }

            using (ISession session = this.OpenSession())
            {
                Component component = null;
                var foundEntity = session.QueryOver<EntityWithComponentsCollection>()
                    .JoinAlias(x => x.Components, () => component, JoinType.LeftOuterJoin)
                    .Where(g => component.Val1 == val1)
                    .SingleOrDefault();

                Assert.IsNotNull(foundEntity);
            }

            using (ISession session = this.OpenSession())
            {
                session.Delete(entity);
                session.Flush();
            }
        }
    }
}
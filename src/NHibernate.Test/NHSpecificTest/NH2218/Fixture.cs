using System.Linq;
using NHibernate.Linq;
using NHibernate.Test.NHSpecificTest.NH0000;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2218
{
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				for (int i = 0; i < 4; i++)
				{
					session.Save("Entity1", new Entity {Name = "Mapping1 -" + i});
				}
				for (int i = 0; i < 3; i++)
				{
					session.Save("Entity2", new Entity {Name = "Mapping2 -" + i});
				}

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void SelectEntitiesByEntityName()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				// verify the instance count for both mappings
				Assert.That(session.Query<Entity>("Entity1").Count(), Is.EqualTo(4));
				Assert.That(session.Query<Entity>("Entity2").Count(), Is.EqualTo(3));

				// verify that all instances are loaded from the right table
				Assert.That(session.Query<Entity>("Entity1").Count(x => x.Name.Contains("Mapping1")), Is.EqualTo(4));
				Assert.That(session.Query<Entity>("Entity2").Count(x => x.Name.Contains("Mapping2")), Is.EqualTo(3));

				// a query for Entity returns instances of both mappings 
				// Remark: session.Query<Entity>().Count() doesn't work because only the count of the first mapping (4) is returned
				Assert.That(session.Query<Entity>().ToList().Count, Is.EqualTo(7));
			}
		}

		[Test]
		public void SelectEntitiesByEntityNameFromStatelessSession()
		{
			using (var session = Sfi.OpenStatelessSession())
			using (session.BeginTransaction())
			{
				// verify the instance count for both mappings
				Assert.That(session.Query<Entity>("Entity1").Count(), Is.EqualTo(4));
				Assert.That(session.Query<Entity>("Entity2").Count(), Is.EqualTo(3));

				// verify that all instances are loaded from the right table
				Assert.That(session.Query<Entity>("Entity1").Count(x => x.Name.Contains("Mapping1")), Is.EqualTo(4));
				Assert.That(session.Query<Entity>("Entity2").Count(x => x.Name.Contains("Mapping2")), Is.EqualTo(3));

				// a query for Entity returns instances of both mappings 
				// Remark: session.Query<Entity>().Count() doesn't work because only the count of the first mapping (4) is returned
				Assert.That(session.Query<Entity>().ToList().Count, Is.EqualTo(7));
			}
		}
	}
}

using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3057
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var a = new AClass { Id = 1 };
				session.Save(a);

				var b = new BClass { Id = 2, A = a, InheritedProperty = "B2" };
				session.Save(b);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");
				transaction.Commit();
			}
		}

		[Test]
		public void CollectionQueryOnJoinedSubclassInheritedProperty()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var entities = session.Query<AClass>()
					.Where(a => a.Bs.Any(b => b.InheritedProperty == "B2"))
					.ToList();

				Assert.AreEqual(1, entities.Count);
				Assert.AreEqual(1, entities[0].Id);
			}
		}

		[Test]
		public void CollectionQueryOnJoinedSubclassInheritedPropertyHql()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var entities = session.CreateQuery("from AClass a where exists (from a.Bs b where b.InheritedProperty = 'B2')")
					.List<AClass>();

				Assert.AreEqual(1, entities.Count);
				Assert.AreEqual(1, entities[0].Id);
			}
		}
	}
}

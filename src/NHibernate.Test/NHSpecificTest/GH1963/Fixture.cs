using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1963
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new EntityChild {Name = "Bob", Flag = true};
				session.Save(e1);

				var e2 = new Entity {Name = "Sally", Child = e1};
				session.Save(e2);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				// The HQL delete does all the job inside the database without loading the entities, but it does
				// not handle delete order for avoiding violating constraints if any. Use
				// session.Delete("from System.Object");
				// instead if in need of having NHibernate ordering the deletes, but this will cause
				// loading the entities in the session.
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void LinqFilterOnCustomType()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = from e in session.Query<Entity>()
							 where e.Child.Flag == true
							 select e;

				Assert.That(result.ToList(), Has.Count.EqualTo(1));
			}
		}
	}
}

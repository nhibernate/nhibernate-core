using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3641
{
	public class TestFixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var child = new Entity {Id = 1, Flag = true};
				var parent = new Entity {Id = 2, ChildInterface = child, ChildConcrete = child};

				session.Save(child);
				session.Save(parent);

				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				DeleteAll<Entity>(session);
				tx.Commit();
			}
		}

		private static void DeleteAll<T>(ISession session)
		{
			session.CreateQuery("delete from " + typeof(T).Name + " where ChildInterface is not null").ExecuteUpdate();
			session.CreateQuery("delete from " + typeof(T).Name).ExecuteUpdate();
		}

		[Test]
		public void TrueOrChildPropertyConcrete()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<IEntity>()
					.Where(x => x.ChildConcrete == null || x.ChildConcrete.Flag)
					.ToList();
				Assert.That(result, Has.Count.EqualTo(2));
			}
		}

		[Test]
		public void TrueOrChildPropertyInterface()
		{
			using (var session = OpenSession())
			{
				var result = session.Query<IEntity>()
					.Where(x => x.ChildInterface == null || ((Entity) x.ChildInterface).Flag)
					.ToList();
				Assert.That(result, Has.Count.EqualTo(2));
			}
		}
	}
}
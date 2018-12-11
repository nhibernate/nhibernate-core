using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1928
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var ez = new EntityZ { Name = "Bob" };
				session.Save(ez);
				var ey = new EntityY { Z = ez };
				session.Save(ey);
				var ex = new EntityX { Y = ey };
				session.Save(ex);

				ez = new EntityZ { Name = "Sally" };
				session.Save(ez);
				ey = new EntityY { Z = ez };
				session.Save(ey);
				ex = new EntityX { Y = ey };
				session.Save(ex);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void JoinAliasOnJoin()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				EntityZ aliasZ = null;
				var result = session
					.QueryOver<EntityX>()
					.Inner.JoinQueryOver(x => x.Y)
					.Inner.JoinAlias(y => y.Z, () => aliasZ)
					.Select(Projections.Entity(() => aliasZ))
					.List<EntityZ>();

				Assert.That(result, Has.Count.EqualTo(2));
			}
		}
	}
}

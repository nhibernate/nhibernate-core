using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1640
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void FetchJoinShouldNotReturnProxyTest()
		{
			int savedId;
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					var sub = new Entity {Id = 2, Name = "Child 2"};
					savedId = (int) session.Save(new Entity {Id = 1, Name = "Parent 1", Child = sub});
					tx.Commit();
				}
			}

			using (IStatelessSession session = Sfi.OpenStatelessSession())
			{
				var parent =
					session.CreateQuery("from Entity p join fetch p.Child where p.Id=:pId").SetInt32("pId", savedId).UniqueResult
						<Entity>();
				Assert.That(parent.Child,Is.TypeOf(typeof (Entity)));
			}

			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Delete("from Entity");
					tx.Commit();
				}
			}
		}
	}
}
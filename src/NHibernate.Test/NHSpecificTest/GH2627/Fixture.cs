using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2627
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Bob" };
				session.Save(e1);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from Child").ExecuteUpdate();
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void NullRefInMerge()
		{
			Child child;
			using (var session = OpenSession())
			using (var t = session.BeginTransaction())
			{
				child = new Child
				{
					Parent = session.Query<Entity>().FirstOrDefault(),
					Name = "John"
				};

				t.Commit();
			}

			using (var session = OpenSession())
			using (var t = session.BeginTransaction())
			{
				session.Merge(child);
				t.Commit();
			}
		}
	}
}

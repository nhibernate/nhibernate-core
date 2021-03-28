using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2053
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Description = "DESCRIPTION", Status = 0 };
				session.Save(e1);

				var e2 = new Entity { Description = "DESCRIPTION", Status = 1 };
				session.Save(e2);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from Entity").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void Test()
		{
			var descriptions = new [] { "DESCRIPTION" };

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var result = session.Query<Entity>()
				                    .Where(x => descriptions.Contains(x.Description))
				                    .Update(x => new Entity {Description = "DESCRIPTION_UPDATED"});
				Assert.That(result, Is.EqualTo(1));

				transaction.Commit();
			}
		}
	}
}

using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3424
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			var c1 = new Child { Name = "Rob" };
			session.Save(c1);
			var e1 = new Entity { Name = "Bob", Children = new HashSet<Child> { c1 } };
			session.Save(e1);

			transaction.Commit();
		}

		protected override void OnTearDown()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();
			session.CreateQuery("delete Child").ExecuteUpdate();
			session.CreateQuery("delete from System.Object").ExecuteUpdate();

			transaction.Commit();
		}

		[Test]
		public void QueryingAfterFutureThenClear()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();
			var futureBob = session.Query<Entity>().Where(e => e.Name == "Bob").ToFutureValue(q => q.FirstOrDefault());
			var bob = futureBob.Value;
			Assert.That(bob, Is.Not.Null);
			session.Clear();

			var allQuery = session.Query<Entity>();
			Assert.That(() => allQuery.ToList(), Has.Count.EqualTo(1));
			transaction.Commit();
		}
	}
}

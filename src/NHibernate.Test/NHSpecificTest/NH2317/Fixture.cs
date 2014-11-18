using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2317
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (var session = sessions.OpenStatelessSession())
			using (var tx = session.BeginTransaction())
			{
				foreach (var artistName in new[] { "Foo", "Bar", "Baz", "Soz", "Tiz", "Fez" })
				{
					session.Insert(new Artist { Name = artistName });
				}
				tx.Commit();
			}
		}

		[Test]
		public void QueryShouldWork()
		{
			using (var session = sessions.OpenSession())
			using(session.BeginTransaction())
			{
				// The HQL : "select a.id from Artist a where a in (from Artist take 3)"
				// shows how should look the HQL tree in the case where Skip/Take are not the last sentences.

				// When the query has no where-clauses the the HQL can be reduced to: "select a.id from Artist a take 3)"

				var expected = session.CreateQuery("select a.id from Artist a take 3").List<int>();
				var actual = session.Query<Artist>().Take(3).Select(a => a.Id).ToArray();
				Assert.That(actual, Is.EquivalentTo(expected));
			}
		}

		protected override void OnTearDown()
		{
			using(var session = sessions.OpenStatelessSession())
			using (var tx = session.BeginTransaction())
			{
				session.CreateQuery("delete Artist").ExecuteUpdate();
				tx.Commit();
			}
			base.OnTearDown();
		}
	}
}
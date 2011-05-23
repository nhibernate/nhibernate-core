using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2317
{
	[TestFixture, Ignore("Not fixed yet.")]
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
				var expected = session.CreateQuery("select a.id from Artist a").SetMaxResults(3).List<int>();
				var actual = session.Query<Artist>().Take(3).Select(a => a.Id).ToArray();
				actual.Should().Have.SameValuesAs(expected);
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
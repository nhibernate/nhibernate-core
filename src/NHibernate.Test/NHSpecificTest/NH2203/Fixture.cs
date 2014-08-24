using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2203
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
				var actual = session.Query<Artist>()
										.OrderBy(a => a.Name)
										.Where(a => a.Name.StartsWith("F"))
										.ToArray();

				actual.Select(a => a.Name).Should().Have.SameSequenceAs("Fez", "Foo");
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
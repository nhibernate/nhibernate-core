using System.Linq;
using NHibernate.DomainModel;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2203
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (var session = Sfi.OpenStatelessSession())
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
			using (var session = Sfi.OpenSession())
			using(session.BeginTransaction())
			{
				var actual = session.Query<Artist>()
										.OrderBy(a => a.Name)
										.Where(a => a.Name.StartsWith("F"))
										.ToArray();

				var expected = new[] {"Fez", "Foo"};
				Assert.That(actual.Select(a => a.Name), Is.EquivalentTo(expected));
			}
		}

		protected override void OnTearDown()
		{
			using(var session = Sfi.OpenStatelessSession())
			using (var tx = session.BeginTransaction())
			{
				session.CreateQuery("delete Artist").ExecuteUpdate();
				tx.Commit();
			}
			base.OnTearDown();
		}
	}
}
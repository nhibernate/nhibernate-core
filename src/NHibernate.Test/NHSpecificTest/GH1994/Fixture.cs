using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1994
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var a = new Asset();
				var d = new Document { IsDeleted = true };
				a.Documents.Add(d);

				session.Save(a);
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				// The HQL delete does all the job inside the database without loading the entities, but it does
				// not handle delete order for avoiding violating constraints if any. Use
				// session.Delete("from System.Object");
				// instead if in need of having NHbernate ordering the deletes, but this will cause
				// loading the entities in the session.

				session.Delete("from System.Object");

				transaction.Commit();
			}
		}

		[Test]
		public void YourTestName()
		{
			using (var s = OpenSession())
			{
				var assetsUnfiltered = s.Query<Asset>()
									.FetchMany(x => x.Documents)
									.ToList();
				
				s.EnableFilter("deletedFilter").SetParameter("deletedParam", false);

				s.Clear();
				var assetsFilteredQuery = s.Query<Asset>()
									.FetchMany(x => x.Documents)
									.ToList();

				s.Clear();
				var assetsFilteredQueryOver = s.QueryOver<Asset>()
									.Fetch(SelectMode.Fetch, x => x.Documents)
									.List<Asset>();

				Assert.That(assetsUnfiltered.Count, Is.EqualTo(1), "unfiltered assets");
				Assert.That(assetsUnfiltered[0].Documents.Count, Is.EqualTo(1), "unfiltered asset documents");

				Assert.That(assetsFilteredQueryOver.Count, Is.EqualTo(1), " query over filtered assets");
				Assert.That(assetsFilteredQueryOver[0].Documents.Count, Is.EqualTo(0), "query over filtered asset documents");

				Assert.That(assetsFilteredQuery.Count, Is.EqualTo(1), "query filtered assets");
				Assert.That(assetsFilteredQuery[0].Documents.Count, Is.EqualTo(0), "query filtered asset documents");
			}
		}
	}
}

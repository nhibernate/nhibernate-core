using NHibernate.Engine.Query;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2614
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Save(new ConcreteClass1 {Name = "C11"});
				s.Save(new ConcreteClass1 {Name = "C12"});
				s.Save(new ConcreteClass2 {Name = "C21"});
				s.Save(new ConcreteClass2 {Name = "C22"});
				s.Save(new ConcreteClass2 {Name = "C23"});
				s.Save(new ConcreteClass2 {Name = "C24"});
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CreateQuery("delete from System.Object").ExecuteUpdate();
				t.Commit();
			}
		}

		private const string PolymorphicQuery =
			"SELECT Name FROM NHibernate.Test.NHSpecificTest.GH2614.BaseClass ROOT";

		[Test]
		public void PolymorphicListReturnsCorrectResults()
		{
			AssertPolymorphicList(PolymorphicQuery, null, 10, expectedCount: 6, expectWarning: false);
		}

		[Test]
		public void PolymorphicListWithMaxResultsSpanningTranslatorsReturnsCorrectResults([Range(0, 6)] int maxResults)
		{
			AssertPolymorphicList(PolymorphicQuery, null, maxResults, expectedCount: maxResults, expectWarning: false);
		}

		[Test]
		public void PolymorphicListWithSkipReturnsCorrectResults()
		{
			AssertPolymorphicList(PolymorphicQuery, 5, 5, expectedCount: 1, expectWarning: true);
		}

		[Test]
		public void PolymorphicListWithSkipSpanningTranslatorsReturnsCorrectResults()
		{
			AssertPolymorphicList(PolymorphicQuery, 1, 4, expectedCount: 4, expectWarning: true);
		}

		[Test]
		public void PolymorphicListWithSkipManyReturnsCorrectResults()
		{
			AssertPolymorphicList(PolymorphicQuery, 6, 5, expectedCount: 0, expectWarning: true);
		}

		[Test]
		public void PolymorphicListWithDuplicatesReturnsCorrectResults([Range(1, 6)] int maxResults)
		{
			// The cross join yields each entity twice, while the polymorphic query plan removes duplicates in memory.
			AssertPolymorphicList(
				"SELECT ROOT FROM NHibernate.Test.NHSpecificTest.GH2614.BaseClass ROOT, NHibernate.Test.NHSpecificTest.GH2614.ConcreteClass1 C",
				null,
				maxResults,
				expectedCount: maxResults,
				expectWarning: false);
		}

		[Test]
		public void PolymorphicListWithOrderByStillShowsWarning()
		{
			AssertPolymorphicList(PolymorphicQuery + " ORDER BY ROOT.Name", null, 3, expectedCount: 3, expectWarning: true);
		}

		private void AssertPolymorphicList(string hql, int? firstResult, int maxResults, int expectedCount, bool expectWarning)
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			using (var logSpy = new LogSpy(typeof(HQLQueryPlan)))
			{
				var query = s.CreateQuery(hql);
				if (firstResult.HasValue)
					query.SetFirstResult(firstResult.Value);
				query.SetMaxResults(maxResults);
				var list = query.List();

				Assert.That(list.Count, Is.EqualTo(expectedCount));
				Assert.That(
					logSpy.GetWholeLog(),
					expectWarning ? Does.Contain("applying in memory") : Does.Not.Contain("applying in memory"));
			}
		}
	}
}

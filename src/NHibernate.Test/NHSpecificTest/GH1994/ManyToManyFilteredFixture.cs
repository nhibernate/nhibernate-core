﻿using System.Linq;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1994
{
	[TestFixture]
	public class ManyToManyFilteredFixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var a = new Asset();
				a.Documents.Add(new Document { IsDeleted = true });
				a.Documents.Add(new Document { IsDeleted = false });

				session.Save(a);
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");
				transaction.Commit();
			}
		}

		[Test]
		public void TestUnfilteredLinqQuery()
		{
			using (var s = OpenSession())
			{
				var query = s.Query<Asset>()
				             .FetchMany(x => x.Documents)
				             .ToList();
				
				Assert.That(query.Count, Is.EqualTo(1), "unfiltered assets");
				Assert.That(query[0].Documents.Count, Is.EqualTo(2), "unfiltered asset documents");
			}
		}

		[Test]
		public void TestFilteredByWhereCollectionLinqQuery()
		{
			using (var s = OpenSession())
			{
				var query = s.Query<Asset>()
				             .FetchMany(x => x.DocumentsFiltered)
				             .ToList();
				
				Assert.That(query.Count, Is.EqualTo(1), "unfiltered assets");
				Assert.That(query[0].DocumentsFiltered.Count, Is.EqualTo(1), "unfiltered asset documents");
			}
		}

		//GH-1994
		[Test]
		public void TestFilteredLinqQuery()
		{
			using (var s = OpenSession())
			{
				s.EnableFilter("deletedFilter").SetParameter("deletedParam", false);
				var query = s.Query<Asset>()
				             .FetchMany(x => x.Documents)
				             .ToList();

				Assert.That(query.Count, Is.EqualTo(1), "filtered assets");
				Assert.That(query[0].Documents.Count, Is.EqualTo(1), "filtered asset documents");
			}
		}

		[Test]
		public void TestFilteredQueryOver()
		{
			using (var s = OpenSession())
			{
				s.EnableFilter("deletedFilter").SetParameter("deletedParam", false);

				var query = s.QueryOver<Asset>()
				             .Fetch(SelectMode.Fetch, x => x.Documents)
				             .TransformUsing(Transformers.DistinctRootEntity)
				             .List<Asset>();

				Assert.That(query.Count, Is.EqualTo(1), "filtered assets");
				Assert.That(query[0].Documents.Count, Is.EqualTo(1), "filtered asset documents");
			}
		}

		[Test]
		public void TestFilteredBagQueryOver()
		{
			using (var s = OpenSession())
			{
				s.EnableFilter("deletedFilter").SetParameter("deletedParam", false);

				var query = s.QueryOver<Asset>()
				             .Fetch(SelectMode.Fetch, x => x.DocumentsBag)
				             .TransformUsing(Transformers.DistinctRootEntity)
				             .List<Asset>();

				Assert.That(query.Count, Is.EqualTo(1), "filtered assets");
				Assert.That(query[0].DocumentsBag.Count, Is.EqualTo(1), "filtered asset documents");
			}
		}

		//NH-2991
		[Test]
		public void TestQueryOverRestrictionWithClause()
		{
			using (var s = OpenSession())
			{
				Document docs = null;
				var query = s.QueryOver<Asset>()
							 .JoinQueryOver(a => a.Documents, () => docs, JoinType.LeftOuterJoin, Restrictions.Where(() => docs.IsDeleted != true))
							 .TransformUsing(Transformers.DistinctRootEntity)
							 .List<Asset>();

				Assert.That(query.Count, Is.EqualTo(1), "filtered assets");
				Assert.That(query[0].Documents.Count, Is.EqualTo(1), "filtered asset documents");
			}
		}

		[Test]
		public void LazyLoad()
		{
			using (var s = OpenSession())
			{
				var asset = s.Query<Asset>().First();
				Assert.That(asset.Documents.Count, Is.EqualTo(2));
				Assert.That(asset.DocumentsBag.Count, Is.EqualTo(2));
				Assert.That(asset.DocumentsFiltered.Count, Is.EqualTo(1));
			}
		}

		[Test]
		public void LazyLoadFiltered()
		{
			using (var s = OpenSession())
			{
				s.EnableFilter("deletedFilter").SetParameter("deletedParam", false);

				var asset = s.Query<Asset>().First();
				Assert.That(asset.Documents.Count, Is.EqualTo(1));
				Assert.That(asset.DocumentsBag.Count, Is.EqualTo(1));
				Assert.That(asset.DocumentsFiltered.Count, Is.EqualTo(1));
			}
		}
	}
}

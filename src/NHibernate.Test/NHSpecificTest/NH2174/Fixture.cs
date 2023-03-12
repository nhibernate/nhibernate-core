using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2174
{
	[TestFixture]
	public class CollectionWithSubclassesAndCompositeIdFixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var doc = new Document {Id_Base = 1, Id_Doc = 2};
				session.Save(doc);
				var detail = new DocumentDetailDocument {Id_Base = 1, Id_Doc = 2, Id_Item = 1, ReferencedDocument = doc};
				session.Save(detail);

				doc.RefferedDetailsManyToMany.Add(detail);
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
		public void LinqFetch()
		{
			using (var session = OpenSession())
			{
				var result = (from e in session.Query<Document>().Fetch(x => x.RefferedDetails)
							select e).FirstOrDefault();

				Assert.That(result.RefferedDetails, Has.Count.EqualTo(1));
			}
		}

		[Test(Description = "GH-3239")]
		public void LinqFetchManyToMany()
		{
			using (var session = OpenSession())
			{
				var result = (from e in session.Query<Document>().Fetch(x => x.RefferedDetailsManyToMany)
							select e).FirstOrDefault();

				Assert.That(result.RefferedDetailsManyToMany, Has.Count.EqualTo(1));
			}
		}

		[Test]
		public void QueryOverFetch()
		{
			using (var session = OpenSession())
			{
				var result = session.QueryOver<Document>().Fetch(SelectMode.Fetch, x => x.RefferedDetails).SingleOrDefault();
				Assert.That(result.RefferedDetails, Has.Count.EqualTo(1));
			}
		}

		[Test(Description = "GH-3239")]
		public void QueryOverFetchManyToMany()
		{
			using (var session = OpenSession())
			{
				var result = session.QueryOver<Document>().Fetch(SelectMode.Fetch, x => x.RefferedDetailsManyToMany).SingleOrDefault();
				Assert.That(result.RefferedDetailsManyToMany, Has.Count.EqualTo(1));
			}
		}

		[Test]
		public void LazyLoad()
		{
			using (var session = OpenSession())
			{
				var result = (from e in session.Query<Document>()
							select e).First();
				Assert.That(result.RefferedDetails.Count, Is.EqualTo(1));
			}
		}
	}
}

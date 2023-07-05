﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2174
{
	using System.Threading.Tasks;
	[TestFixture]
	public class CollectionWithSubclassesAndCompositeIdFixtureAsync : BugTestCase
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
		public async Task LinqFetchAsync()
		{
			using (var session = OpenSession())
			{
				var result = await ((from e in session.Query<Document>().Fetch(x => x.RefferedDetails)
							select e).FirstOrDefaultAsync());

				Assert.That(result.RefferedDetails, Has.Count.EqualTo(1));
			}
		}

		[Test(Description = "GH-3239")]
		public async Task LinqFetchManyToManyAsync()
		{
			using var session = OpenSession();
			var result = await (session.Query<Document>().Fetch(x => x.RefferedDetailsManyToMany).FirstAsync());
			Assert.That(result.RefferedDetailsManyToMany, Has.Count.EqualTo(1));
		}

		[Test]
		public async Task QueryOverFetchAsync()
		{
			using (var session = OpenSession())
			{
				var result = await (session.QueryOver<Document>().Fetch(SelectMode.Fetch, x => x.RefferedDetails).SingleOrDefaultAsync());
				Assert.That(result.RefferedDetails, Has.Count.EqualTo(1));
			}
		}

		[Test(Description = "GH-3239")]
		public async Task QueryOverFetchManyToManyAsync()
		{
			using var session = OpenSession();
			var result = await (session.QueryOver<Document>().Fetch(SelectMode.Fetch, x => x.RefferedDetailsManyToMany).SingleOrDefaultAsync());
			Assert.That(result.RefferedDetailsManyToMany, Has.Count.EqualTo(1));
		}

		[Test]
		public async Task LazyLoadAsync()
		{
			using (var session = OpenSession())
			{
				var result = await ((from e in session.Query<Document>()
							select e).FirstAsync());
				Assert.That(result.RefferedDetails.Count, Is.EqualTo(1));
			}
		}

		[Test]
		public async Task LazyLoadManyToManyAsync()
		{
			using var session = OpenSession();
			var result = await (session.Query<Document>().FirstAsync());
			Assert.That(result.RefferedDetailsManyToMany.Count, Is.EqualTo(1));
		}
	}
}

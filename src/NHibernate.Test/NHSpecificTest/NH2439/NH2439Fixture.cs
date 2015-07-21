using System;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2439
{
	[TestFixture]
	public class NH2439Fixture : BugTestCase
	{
		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SetProperty(Cfg.Environment.ShowSql, "true");
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			{
				var organisation = new Organisation();
				var trainingComponent = new TrainingComponent {Code = "123", Title = "title"};
				var scope = new RtoScope
								{
									Nrt = trainingComponent,
									Rto = organisation,
									StartDate = DateTime.Today.AddDays(-100)
								};

				session.Save(organisation);
				session.Save(trainingComponent);
				session.Save(scope);

				var searchResult = new OrganisationSearchResult {Organisation = organisation};
				session.Save(searchResult);
				session.Flush();
			}
			base.OnSetUp();
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			{
				session.Delete("from OrganisationSearchResult");
				session.Delete("from RtoScope");
				session.Delete("from Organisation");
				session.Delete("from TrainingComponent");
				session.Flush();
			}
		}

		[Test]
		public void TheTest()
		{
			using (var session = OpenSession())
			{
				const string filter = "ABC";

				var scopes = session.Query<RtoScope>()
					.Where(s => s.StartDate <= DateTime.Today && (s.EndDate == null || s.EndDate >= DateTime.Today) && !s.IsRefused)
					.Where(t => t.Nrt.Title.Contains(filter) || t.Nrt.Code == filter);

				var query = session.Query<OrganisationSearchResult>();
				var finalQuery = query.Where(r => scopes.Any(s => s.Rto == r.Organisation));
				var organisations = scopes.Select(s => s.Rto);

				var rtoScopes = scopes.ToList();
				var organisations1 = organisations.ToList();

				Assert.That(rtoScopes.Count == 0);
				Assert.That(organisations1.Count == 0);

				//the SQL in below fails - the organisations part of the query is not included at all..
				var organisationSearchResults = query
					.Where(r => organisations.Contains(r.Organisation))
					.ToList();

				Assert.That(organisationSearchResults.Count == 0);

				var list = finalQuery.ToList();
				Assert.That(list.Count == 0);
			}
		}
	}
}

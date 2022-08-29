using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1879
{
	[TestFixture]
	public class ConditionalThenAccessMember : GH1879BaseFixture<Project>
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var clientA = new Client { Name = "Albert" };
				var clientB = new Client { Name = "Bob" };
				var clientC = new CorporateClient { Name = "Charlie", CorporateId = "1234" };
				session.Save(clientA);
				session.Save(clientB);
				session.Save(clientC);

				session.Save(new Project { Name = "A", EmailPref = EmailPref.Primary, Client = clientA, BillingClient = clientB, CorporateClient = clientC, });
				session.Save(new Project { Name = "B", EmailPref = EmailPref.Billing, Client = clientA, BillingClient = clientB, CorporateClient = clientC, });
				session.Save(new Project { Name = "C", EmailPref = EmailPref.Corp, Client = clientA, BillingClient = clientB, CorporateClient = clientC, });

				session.Save(new Project { Name = "D", EmailPref = EmailPref.Primary, Client = null, BillingClient = clientB, CorporateClient = clientC, });
				session.Save(new Project { Name = "E", EmailPref = EmailPref.Billing, Client = clientA, BillingClient = null, CorporateClient = clientC, });
				session.Save(new Project { Name = "F", EmailPref = EmailPref.Corp, Client = clientA, BillingClient = clientB, CorporateClient = null, });

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void WhereClause()
		{
			AreEqual(
				// Actual
				q => q.Where(p => (p.EmailPref == EmailPref.Primary
								 ? p.Client
								 : p.EmailPref == EmailPref.Corp
									 ? p.CorporateClient
									 : p.BillingClient).Name.Length > 3),
				// Expected
				q => q.Where(p => (p.EmailPref == EmailPref.Primary
								 ? p.Client.Name
								 : p.EmailPref == EmailPref.Corp
									 ? p.CorporateClient.Name
									 : p.BillingClient.Name).Length > 3)
			);
		}

		[Test]
		public void SelectClause()
		{
			AreEqual(
				// Actual
				q => q.OrderBy(p => p.Name)
					  .Select(p => (p.EmailPref == EmailPref.Primary
								  ? p.Client
								  : p.EmailPref == EmailPref.Corp
									  ? p.CorporateClient
									  : p.BillingClient).Name),
				// Expected
				q => q.OrderBy(p => p.Name)
					  .Select(p => p.EmailPref == EmailPref.Primary
								  ? p.Client.Name
								  : p.EmailPref == EmailPref.Corp
									  ? p.CorporateClient.Name
									  : p.BillingClient.Name)
			);
		}

		[Test]
		public void SelectClauseToAnon()
		{
			AreEqual(
				// Actual
				q => q.OrderBy(p => p.Name)
					  .Select(p => new
					  {
						  p.Name,
						  Client = (p.EmailPref == EmailPref.Primary
						  ? p.Client
						  : p.EmailPref == EmailPref.Corp
							  ? p.CorporateClient
							  : p.BillingClient).Name
					  }),
				// Expected
				q => q.OrderBy(p => p.Name)
					  .Select(p => new
					  {
						  p.Name,
						  Client = p.EmailPref == EmailPref.Primary
						  ? p.Client.Name
						  : p.EmailPref == EmailPref.Corp
							  ? p.CorporateClient.Name
							  : p.BillingClient.Name
					  })
			);
		}

		[Test]
		public void OrderByClause()
		{
			AreEqual(
				// Actual
				q => q.OrderBy(p => (p.EmailPref == EmailPref.Primary
								   ? p.Client
								   : p.EmailPref == EmailPref.Corp
									   ? p.CorporateClient
									   : p.BillingClient).Name ?? "ZZZ")
					  .ThenBy(p => p.Name)
					  .Select(p => p.Name),
				// Expected
				q => q.OrderBy(p => (p.EmailPref == EmailPref.Primary
								   ? p.Client.Name
								   : p.EmailPref == EmailPref.Corp
									   ? p.CorporateClient.Name
									   : p.BillingClient.Name) ?? "ZZZ")
					  .ThenBy(p => p.Name)
					  .Select(p => p.Name)
			);
		}

		[Test]
		public void GroupByClause()
		{
			AreEqual(
				// Actual
				q => q.GroupBy(p => (p.EmailPref == EmailPref.Primary
								   ? p.Client
								   : p.EmailPref == EmailPref.Corp
									   ? p.CorporateClient
									   : p.BillingClient).Name)
					  .OrderBy(x => x.Key ?? "ZZZ")
					  .Select(grp => new { grp.Key, Count = grp.Count() }),
				// Expected
				q => q.GroupBy(p => p.EmailPref == EmailPref.Primary
								   ? p.Client.Name
								   : p.EmailPref == EmailPref.Corp
									   ? p.CorporateClient.Name
									   : p.BillingClient.Name)
					  .OrderBy(x => x.Key ?? "ZZZ")
					  .Select(grp => new { grp.Key, Count = grp.Count() })
			);
		}
	}
}

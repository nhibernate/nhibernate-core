using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1879
{
	[TestFixture]
	public class CoalesceSiblingsThenAccessMember : GH1879BaseFixture<Project>
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var clientA = new Client { Name = "Albert" };
				var clientB = new Client { Name = "Bob" };
				var corpA = new CorporateClient { Name = "Alpha", CorporateId = "1234" };
				var corpB = new CorporateClient { Name = "Beta", CorporateId = "5647" };
				var clientZ = new Client { Name = null }; // A null value should propagate if the entity is non-null
				session.Save(clientA);
				session.Save(clientB);
				session.Save(corpA);
				session.Save(corpB);
				session.Save(clientZ);
				
				session.Save(new Project { Name = "A", BillingClient = null, CorporateClient = null, Client = clientA });
				session.Save(new Project { Name = "B", BillingClient = null, CorporateClient = null, Client = clientB });
				session.Save(new Project { Name = "C", BillingClient = null, CorporateClient = corpA, Client = clientA });
				session.Save(new Project { Name = "D", BillingClient = null, CorporateClient = corpB, Client = clientA });
				session.Save(new Project { Name = "E", BillingClient = corpA, CorporateClient = null, Client = clientA });
				session.Save(new Project { Name = "F", BillingClient = clientB, CorporateClient = null, Client = clientA });
				session.Save(new Project { Name = "G", BillingClient = clientZ, CorporateClient = null, Client = clientA });
				session.Save(new Project { Name = "Z", BillingClient = null, CorporateClient = null, Client = null });
				
				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void WhereClause()
		{
			AreEqual(
				// Actual
				q => q.Where(p => (p.BillingClient ?? p.CorporateClient ?? p.Client).Name.StartsWith("A")),
				// Expected
				q => q.Where(p => (p.BillingClient != null ? p.BillingClient.Name : p.CorporateClient != null ? p.CorporateClient.Name : p.Client.Name).StartsWith("A"))
			);
		}

		[Test]
		public void SelectClause()
		{
			AreEqual(
				// Actual
				q => q.OrderBy(p => p.Name)
				      .Select(p => (p.BillingClient ?? p.CorporateClient ?? p.Client).Name),
				// Expected
				q => q.OrderBy(p => p.Name)
				      .Select(p => p.BillingClient != null ? p.BillingClient.Name : p.CorporateClient != null ? p.CorporateClient.Name : p.Client.Name)
			);
		}
		
		[Test]
		public void SelectClauseToAnon()
		{
			AreEqual(
				// Actual
				q => q.OrderBy(p => p.Name)
				      .Select(p => new { Project = p.Name, Client = (p.BillingClient ?? p.CorporateClient ?? p.Client).Name }),
				// Expected
				q => q.OrderBy(p => p.Name)
				      .Select(p => new { Project = p.Name, Client = p.BillingClient != null ? p.BillingClient.Name : p.CorporateClient != null ? p.CorporateClient.Name : p.Client.Name })
			);
		}

		[Test]
		public void OrderByClause()
		{
			AreEqual(
				// Actual
				q => q.OrderBy(p => (p.BillingClient ?? p.CorporateClient ?? p.Client).Name ?? "ZZZ")
				      .ThenBy(p => p.Name)
				      .Select(p => p.Name),
				// Expected
				q => q.OrderBy(p => (p.BillingClient != null ? p.BillingClient.Name : p.CorporateClient != null ? p.CorporateClient.Name : p.Client.Name) ?? "ZZZ")
				      .ThenBy(p => p.Name)
				      .Select(p => p.Name)
			);
		}

		[Test]
		public void GroupByClause()
		{
			AreEqual(
				// Actual
				q => q.GroupBy(p => (p.BillingClient ?? p.CorporateClient ?? p.Client).Name)
				      .OrderBy(x => x.Key ?? "ZZZ")
				      .Select(grp => new  { grp.Key, Count = grp.Count() }),
				// Expected
				q => q.GroupBy(p => p.BillingClient != null ? p.BillingClient.Name : p.CorporateClient != null ? p.CorporateClient.Name : p.Client.Name)
				      .OrderBy(x => x.Key ?? "ZZZ")
				      .Select(grp => new  { grp.Key, Count = grp.Count() })
			);
		}
	}
}

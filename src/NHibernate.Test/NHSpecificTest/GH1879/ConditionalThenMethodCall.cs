using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1879
{
	[TestFixture]
	public class ConditionalThenMethodCall : GH1879BaseFixture<Employee>
	{
		/// <inheritdoc />
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var clientA = new Client { Name = "Alpha" };
				var clientB = new Client { Name = "Beta" };
				session.Save(clientA);
				session.Save(clientB);

				var issue1 = new Issue { Name = "1", Client = null };
				var issue2 = new Issue { Name = "2", Client = clientA };
				var issue3 = new Issue { Name = "3", Client = clientA };
				var issue4 = new Issue { Name = "4", Client = clientA };
				var issue5 = new Issue { Name = "5", Client = clientB };
				session.Save(issue1);
				session.Save(issue2);
				session.Save(issue3);
				session.Save(issue4);
				session.Save(issue5);

				session.Save(new Employee { Name = "Andy", ReviewAsPrimary = true, ReviewIssues = { issue1, issue2, issue5 }, WorkIssues = { issue3 } });
				session.Save(new Employee { Name = "Bart", ReviewAsPrimary = false, ReviewIssues = { issue3 }, WorkIssues = { issue4, issue5 } });
				session.Save(new Employee { Name = "Carl", ReviewAsPrimary = true, ReviewIssues = { issue3 }, WorkIssues = { issue1, issue4, issue5 } });
				session.Save(new Employee { Name = "Dorn", ReviewAsPrimary = false, ReviewIssues = { issue3 }, WorkIssues = { issue1, issue4 } });

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void WhereClause()
		{
			AreEqual(
				// Conditional style
				q => q.Where(e => (e.ReviewAsPrimary ? e.ReviewIssues : e.WorkIssues).Any(i => i.Client.Name == "Beta")),
				// Expected
				q => q.Where(e => e.ReviewAsPrimary ? e.ReviewIssues.Any(i => i.Client.Name == "Beta") : e.WorkIssues.Any(i => i.Client.Name == "Beta"))
			);
		}

		[Test]
		public void SelectClause()
		{
			AreEqual(
				// Conditional style
				q => q.OrderBy(e => e.Name)
					  .Select(e => (e.ReviewAsPrimary ? e.ReviewIssues : e.WorkIssues).Any(i => i.Client.Name == "Beta")),
				// Expected
				q => q.OrderBy(e => e.Name)
					  .Select(e => e.ReviewAsPrimary ? e.ReviewIssues.Any(i => i.Client.Name == "Beta") : e.WorkIssues.Any(i => i.Client.Name == "Beta"))
			);
		}

		[Test]
		public void SelectClauseToAnon()
		{
			AreEqual(
				// Conditional style
				q => q.OrderBy(e => e.Name)
					  .Select(e => new { e.Name, Beta = (e.ReviewAsPrimary ? e.ReviewIssues : e.WorkIssues).Any(i => i.Client.Name == "Beta") }),
				// Expected
				q => q.OrderBy(e => e.Name)
					  .Select(e => new { e.Name, Beta = e.ReviewAsPrimary ? e.ReviewIssues.Any(i => i.Client.Name == "Beta") : e.WorkIssues.Any(i => i.Client.Name == "Beta") })
			);
		}

		[Test]
		public void OrderByClause()
		{
			AreEqual(
				// Conditional style
				q => q.OrderBy(e => (e.ReviewAsPrimary ? e.ReviewIssues : e.WorkIssues).Count())
					  .ThenBy(p => p.Name)
					  .Select(p => p.Name),
				// Expected
				q => q.OrderBy(e => e.ReviewAsPrimary ? e.ReviewIssues.Count() : e.WorkIssues.Count())
					  .ThenBy(p => p.Name)
					  .Select(p => p.Name)
			);
		}

		[Test]
		public void GroupByClause()
		{
			AreEqual(
				// Conditional style
				q => q.GroupBy(e => (e.ReviewAsPrimary ? e.ReviewIssues : e.WorkIssues).Count())
					  .OrderBy(x => x.Key)
					  .Select(grp => grp.Count()),
				// Expected
				q => q.GroupBy(e => e.ReviewAsPrimary ? e.ReviewIssues.Count() : e.WorkIssues.Count())
					  .OrderBy(x => x.Key)
					  .Select(grp => grp.Count())
			);
		}
	}
}

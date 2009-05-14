using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1776
{
	[TestFixture, Ignore("Not fixed yet.")]
	public class FilterQueryTwiceFixture : BugTestCase
	{
		[Test]
		[Description("Can Query using Session's filter Twice")]
		public void Bug()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					var c = new Category {Code = "2600", Deleted = false};
					s.SaveOrUpdate(c);
					tx.Commit();
				}
			}

			// exec queries, twice, different session
			ExecQuery();
			ExecQuery();

			// cleanup
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					s.EnableFilter("state").SetParameter("deleted", false);
					s.Delete("from Category");
					tx.Commit();
				}
			}
		}

		private void ExecQuery()
		{
			using (ISession s = OpenSession())
			{
				s.EnableFilter("state").SetParameter("deleted", false);

				IList<Category> result =
					s.CreateQuery("from Category where Code = :code").SetParameter("code", "2600").List<Category>();

				Assert.That(result.Count > 0);
			}
		}
	}
}
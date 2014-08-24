using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1776
{
	[TestFixture]
	public class FilterQueryTwiceFixture : BugTestCase
	{
		// Note : in this test what is really important is the usage of the same HQL
		// because QueryPlan

		[Test]
		[Description("Can Query using Session's filter Twice")]
		public void Bug()
		{
			var c = new Category { Code = "2600", Deleted = false };
			SaveCategory(c);

			// exec queries, twice, different session
			ExecQuery();
			ExecQuery();

			// cleanup using filter
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

		private void SaveCategory(Category c)
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					s.Save(c);
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

		[Test]
		[Description("Executing same query with and without filter and with different filter parameter value.")]
		public void FilterOnOffOn()
		{
			var c = new Category { Code = "2600", Deleted = true };
			SaveCategory(c);

			using (ISession s = OpenSession())
			{
				s.EnableFilter("state").SetParameter("deleted", false);

				IList<Category> result =
					s.CreateQuery("from Category where Code = :code").SetParameter("code", "2600").List<Category>();

				Assert.That(result.Count == 0);
			}

			using (ISession s = OpenSession())
			{
				IList<Category> result =
					s.CreateQuery("from Category where Code = :code").SetParameter("code", "2600").List<Category>();

				Assert.That(result.Count > 0);
			}

			using (ISession s = OpenSession())
			{
				s.EnableFilter("state").SetParameter("deleted", true);

				IList<Category> result =
					s.CreateQuery("from Category where Code = :code").SetParameter("code", "2600").List<Category>();

				Assert.That(result.Count > 0);
			}

			Cleanup();
		}

		private void Cleanup()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.CreateQuery("delete from Category").ExecuteUpdate();
				tx.Commit();
			}
		}

		[Test]
		[Description("Executing same query with different filters combinations.")]
		public void MultiFilterOnOffOn()
		{
			var c = new Category { Code = "2600", Deleted = true };
			SaveCategory(c);

			using (ISession s = OpenSession())
			{
				s.EnableFilter("state").SetParameter("deleted", false);

				IList<Category> result =
					s.CreateQuery("from Category where Code = :code").SetParameter("code", "2600").List<Category>();

				Assert.That(result.Count == 0);
			}

			using (ISession s = OpenSession())
			{
				s.EnableFilter("state").SetParameter("deleted", true);
				s.EnableFilter("CodeLike").SetParameter("codepattern", "2%");

				IList<Category> result =
					s.CreateQuery("from Category where Code = :code").SetParameter("code", "NotExists").List<Category>();

				Assert.That(result.Count == 0);
			}

			using (ISession s = OpenSession())
			{
				s.EnableFilter("CodeLike").SetParameter("codepattern", "2%");

				IList<Category> result =
					s.CreateQuery("from Category where Code = :code").SetParameter("code", "2600").List<Category>();

				Assert.That(result.Count > 0);
			}
			Cleanup();
		}
	}
}
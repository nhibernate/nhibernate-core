using System.Collections.Generic;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1792
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			DeleteAll();
		}

		/// <summary>
		/// Deletes all the product entities from the persistence medium
		/// </summary>
		private void DeleteAll()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction trans = session.BeginTransaction())
				{
					session.Delete("from Product");
					trans.Commit();
				}
			}
		}

		/// <summary>
		/// Creates some product enties to work with
		/// </summary>
		protected override void OnSetUp()
		{
			base.OnSetUp();

			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					for (int i = 0; i < 10; i++)
					{
						var prod = new Product {Name = "Product" + i};

						session.Save(prod);
					}
					tx.Commit();
				}
			}
		}

		/// <summary>
		/// Verifies that a subquery created as a detached criteria with an order by 
		/// will produce valid sql when the main query does not contain an order by clause
		/// </summary>
		[Test]
		public void PageWithDetachedCriteriaSubqueryWithOrderBy()
		{
			//create the subquery
			DetachedCriteria subQuery =
				DetachedCriteria.For<Product>().SetProjection(Projections.Id()).AddOrder(Order.Desc("Name")).SetMaxResults(5);

			using (ISession session = OpenSession())
			{
				IList<Product> results =
					session.CreateCriteria<Product>().Add(Subqueries.PropertyIn("Id", subQuery)).Add(Restrictions.Gt("Id", 0)).
						SetMaxResults(3).List<Product>();

				Assert.AreEqual(3, results.Count);
			}
		}

		/// <summary>
		/// Verifies that a subquery created as a raw sql statement with an order by 
		/// will produce valid sql when the main query does not contain an order by clause
		/// </summary>
		[Test]
		public void PageWithRawSqlSubqueryWithOrderBy()
		{
			using (ISession session = OpenSession())
			{
				string top = "";
				if (Dialect.GetType().Name.StartsWith("MsSql"))
					top = "top 5";

				IList<Product> results =
					session.CreateCriteria<Product>().Add(
						Expression.Sql("{alias}.Id in (Select " + top + " p.Id from Product p order by Name)")).Add(Restrictions.Gt("Id", 0)).
						SetMaxResults(3).List<Product>();

				Assert.AreEqual(3, results.Count);
			}
		}
	}
}
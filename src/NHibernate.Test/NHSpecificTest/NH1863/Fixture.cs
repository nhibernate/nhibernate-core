using System.Collections.Generic;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1863
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					var category = new Category {IsActive = true};
					s.Save(category);

					var customerWithCategory = new Customer {Name = "HasCategory"};
					customerWithCategory.Categories.Add(category);
					s.Save(customerWithCategory);

					var customerWithNoCategory = new Customer {Name = "HasNoCategory"};
					s.Save(customerWithNoCategory);

					tx.Commit();
				}
			}
		}

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					s.Delete("from Customer");
					s.Delete("from Category");
					tx.Commit();
				}
			}
		}

		[Test]
		public void CanGetCustomerWithCategoryWhenFilterIsEnabled()
		{
			using (ISession session = OpenSession())
			{
				IFilter filter = session.EnableFilter("onlyActive");
				filter.SetParameter("activeFlag", true);

				ICriteria hasCategoryCriteria = session.CreateCriteria(typeof (Customer));
				hasCategoryCriteria.Add(Restrictions.Eq("Name", "HasCategory"));
				IList<Customer> hasCategoryResult = hasCategoryCriteria.List<Customer>();

				Assert.That(hasCategoryResult.Count, Is.EqualTo(1));
			}

		}

		[Test]
		public void CanGetCustomerWithCategoryWhenFilterIsDisabled()
		{
			using (ISession session = OpenSession())
			{
				session.DisableFilter("onlyActive");

				ICriteria hasCategoryCriteria = session.CreateCriteria(typeof (Customer));
				hasCategoryCriteria.Add(Restrictions.Eq("Name", "HasCategory"));
				IList<Customer> hasCategoryResult = hasCategoryCriteria.List<Customer>();

				Assert.That(hasCategoryResult.Count, Is.EqualTo(1));
			}
		}

		[Test]
		public void CanGetCustomerWithNoCategoryWhenFilterIsEnabled()
		{
			using (ISession session = OpenSession())
			{
				IFilter filter = session.EnableFilter("onlyActive");
				filter.SetParameter("activeFlag", true);

				ICriteria hasNoCategoryCriteria = session.CreateCriteria(typeof (Customer));
				hasNoCategoryCriteria.Add(Restrictions.Eq("Name", "HasNoCategory"));
				IList<Customer> hasNoCategoryResult = hasNoCategoryCriteria.List<Customer>();

				Assert.That(hasNoCategoryResult.Count, Is.EqualTo(1));
			}

		}

		[Test]
		public void CanGetCustomerWithNoCategoryWhenFilterIsDisabled()
		{
			using (ISession session = OpenSession())
			{
				session.DisableFilter("onlyActive");

				ICriteria hasNoCategoryCriteria = session.CreateCriteria(typeof (Customer));
				hasNoCategoryCriteria.Add(Restrictions.Eq("Name", "HasNoCategory"));
				IList<Customer> hasNoCategoryResult = hasNoCategoryCriteria.List<Customer>();

				Assert.That(hasNoCategoryResult.Count, Is.EqualTo(1));
			}

		}
	}
}

using System.Collections.Generic;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1490
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override System.Collections.IList Mappings
		{
			get
			{
				if (Dialect is PostgreSQLDialect)
					return new[] { "NHSpecificTest.NH1490.MappingsFilterAsBoolean.hbm.xml" };

				return base.Mappings;
			}
		}

		[Test]
		public void Can_Translate_Correctly_Without_Filter()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Customer c = new Customer("Somebody");
				c.Category = new Category("User");
				c.IsActive = true;
				c.Category.IsActive = true;
				s.Save(c.Category);
				s.Save(c);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			{
				IQuery query = s.CreateQuery("from Customer c where c.Category.Name = :catName");
				query.SetParameter("catName", "User");
				IList<Customer> customers = query.List<Customer>();

				Assert.That(customers.Count, Is.EqualTo(1), "Can apply condition on Customer without IFilter");
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from Customer");
				s.Delete("from Category");
				tx.Commit();
			}
		}

		[Test]
		public void Also_Works_With_Filter()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Customer c = new Customer("Somebody");
				c.Category = new Category("User");
				s.Save(c.Category);
				c.IsActive = true;
				c.Category.IsActive = true;
				s.Save(c);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			{
				s.DisableFilter("onlyActive");
				IFilter fltr = s.EnableFilter("onlyActive");
				
				if (Dialect is PostgreSQLDialect)
					fltr.SetParameter("activeFlag", true);
				else
					fltr.SetParameter("activeFlag", 1);

				// Customer is parametrized
				IQuery query = s.CreateQuery("from Customer c where c.Name = :customerName");
				query.SetParameter("customerName", "Somebody");
				IList<Customer> customers = query.List<Customer>();

				Assert.That(customers.Count, Is.EqualTo(1), "IFilter applied and Customer parametrized on Name also works");
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from Customer");
				s.Delete("from Category");
				tx.Commit();
			}
		}

		[Test]
		public void Incorrect_SQL_Translated_Params_Bug()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Customer c = new Customer("Somebody");
				c.Category = new Category("User");
				s.Save(c.Category);
				c.IsActive = true;
				c.Category.IsActive = true;
				s.Save(c);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			{
				s.DisableFilter("onlyActive");
				IFilter fltr = s.EnableFilter("onlyActive");
				if (Dialect is PostgreSQLDialect)
					fltr.SetParameter("activeFlag", true);
				else
					fltr.SetParameter("activeFlag", 1);

				// related entity Customer.Category is parametrized
				IQuery query = s.CreateQuery("from Customer c where c.Category.Name = :catName");
				query.SetParameter("catName", "User");

				IList<Customer> customers = query.List<Customer>();

				Assert.That(customers.Count, Is.EqualTo(1), "IFIlter applied and Customer parametrized on Category.Name DOES NOT work");
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from Customer");
				s.Delete("from Category");
				tx.Commit();
			}
		}
	}
}
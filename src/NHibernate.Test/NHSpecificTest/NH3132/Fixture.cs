using System.Collections;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3132
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		/// <summary>
		/// push some data into the database
		/// Really functions as a save test also 
		/// </summary>
		protected override void OnSetUp()
		{
			base.OnSetUp();

			using (var session = OpenSession())
			{
				using (var tran = session.BeginTransaction())
				{
					Product product = new Product();
					product.Name = "First";
					product.Lazy = "Lazy";
					
					session.Save(product);

					tran.Commit();
				}
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();

			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				session.Delete("from Product");
				tran.Commit();
			}
		}

		[Test]
		public void Query_returns_correct_name()
		{
			using (var session = OpenSession())
			{
				Product product = session.CreateCriteria(typeof (Product))
					.Add(Restrictions.Eq("Name", "First"))
					.UniqueResult<Product>();

				Assert.IsNotNull(product);
				Assert.AreEqual("First", product.Name);
			}
		}

		[Test]
		public void Correct_value_gets_saved()
		{
			using (var session = OpenSession())
			{
				var product = session.CreateCriteria(typeof(Product))
					.Add(Restrictions.Eq("Name", "First"))
					.UniqueResult<Product>();

				Assert.That(product, Is.Not.Null);
				product.Name = "Changed";

				session.Flush();
				
				session.Clear();

				var product1 = session.CreateCriteria(typeof(Product))
					.Add(Restrictions.Eq("Name", "Changed"))
					.UniqueResult<Product>();

				Assert.That(product1, Is.Not.Null);
				Assert.That(product1.Name, Is.EqualTo("Changed"));
			}
		}

		[Test]
		public void Correct_value_gets_saved_with_lazy()
		{
			using (var session = OpenSession())
			{
				var product = session.CreateCriteria(typeof(Product))
					.Add(Restrictions.Eq("Name", "First"))
					.UniqueResult<Product>();

				Assert.That(product, Is.Not.Null);
				product.Name = "Changed";
				product.Lazy = "LazyChanged";

				session.Flush();
				
				session.Clear();

				var product1 = session.CreateCriteria(typeof(Product))
					.Add(Restrictions.Eq("Name", "Changed"))
					.UniqueResult<Product>();

				Assert.That(product1, Is.Not.Null);
				Assert.That(product1.Name, Is.EqualTo("Changed"));
				Assert.That(product1.Lazy, Is.EqualTo("LazyChanged"));
			}
		}
	}
}

using System.Collections;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3132
{
	[TestFixture]
	public class Fixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get
			{
				return new string[]
					{
						"NHSpecificTest.NH3132.Mappings.hbm.xml"
					};
			}
		}

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
			{
				using (var tran = session.BeginTransaction())
				{
					session.Delete("from Product");
					tran.Commit();
				}
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
				Product product = session.CreateCriteria(typeof(Product))
					.Add(Restrictions.Eq("Name", "First"))
					.UniqueResult<Product>();

				Assert.IsNotNull(product);
				product.Name = "Changed";

				session.Flush();
				
				session.Clear();

				Product product1 = session.CreateCriteria(typeof(Product))
					.Add(Restrictions.Eq("Name", "Changed"))
					.UniqueResult<Product>();
				
				Assert.IsNotNull(product1);
				Assert.AreEqual("Changed", product1.Name);
			}
		}
	}
}

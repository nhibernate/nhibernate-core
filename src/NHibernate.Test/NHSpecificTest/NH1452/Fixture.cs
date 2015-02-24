using NHibernate.Cfg;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1452
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SetProperty(Environment.FormatSql, "false");
		}

		/// <summary>
		/// push some data into the database
		/// Really functions as a save test also 
		/// </summary>
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				session.Save(new Product
					{
						ProductId = "XO1234",
						Id = 1,
						Name = "Some product",
						Description = "Very good"
					});

				session.Save(new Product
					{
						ProductId = "XO54321",
						Id = 2,
						Name = "Other product",
						Description = "Very bad"
					});

				tran.Commit();
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
		public void Delete_single_record()
		{
			using (var session = OpenSession())
			{
				var product = new Product
					{
						ProductId = "XO1111",
						Id = 3,
						Name = "Test",
						Description = "Test"
					};

				session.Save(product);

				session.Flush();

				session.Delete(product);
				session.Flush();

				session.Clear();

				//try to query for this product
				product = session.CreateCriteria(typeof (Product))
								 .Add(Restrictions.Eq("ProductId", "XO1111"))
								 .UniqueResult<Product>();

				Assert.That(product, Is.Null);
			}
		}

		[Test]
		public void Query_records()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				var product = session.CreateCriteria(typeof (Product))
									 .Add(Restrictions.Eq("ProductId", "XO1234"))
									 .UniqueResult<Product>();

				Assert.That(product, Is.Not.Null);
				Assert.That(product.Description, Is.EqualTo("Very good"));

				var log = sqlLog.GetWholeLog();
				//needs to be joining on the Id column not the productId
				Assert.That(log.Contains("inner join ProductLocalized this_1_ on this_.Id=this_1_.Id"), Is.True);
			}
		}

		[Test]
		public void Update_record()
		{
			using (var session = OpenSession())
			{
				var product = session.CreateCriteria(typeof (Product))
									 .Add(Restrictions.Eq("ProductId", "XO1234"))
									 .UniqueResult<Product>();

				Assert.That(product, Is.Not.Null);

				product.Name = "TestValue";
				product.Description = "TestValue";

				session.Flush();
				session.Clear();

				//pull again
				product = session.CreateCriteria(typeof (Product))
								 .Add(Restrictions.Eq("ProductId", "XO1234"))
								 .UniqueResult<Product>();

				Assert.That(product, Is.Not.Null);
				Assert.That(product.Name, Is.EqualTo("TestValue"));
				Assert.That(product.Description, Is.EqualTo("TestValue"));
			}
		}
	}
}

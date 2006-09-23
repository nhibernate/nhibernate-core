using System;
using System.Collections;
using Iesi.Collections;
using log4net;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Persister.Collection;
using NHibernate.Transform;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.FilterTest
{
	[TestFixture, Ignore("Filters not yet implemented")]
	public class DynamicFilterTest : TestCase
	{
		private ILog log = LogManager.GetLogger(typeof(DynamicFilterTest));

		[Test]
		public void SecondLevelCachedCollectionsFiltering()
		{
			TestData testData = new TestData(this);
			testData.Prepare();

			ISession session = OpenSession();

			// Force a collection into the second level cache, with its non-filtered elements
			Salesperson sp = (Salesperson) session.Load(typeof(Salesperson), testData.steveId);
			NHibernateUtil.Initialize(sp.Orders);
			ICollectionPersister persister = ((ISessionFactoryImplementor) sessions)
					.GetCollectionPersister(typeof(Salesperson).FullName + ".Orders");
			Assert.IsTrue(persister.HasCache, "No cache for collection");
			object[] cachedData = (object[]) persister.Cache.Cache.Get(testData.steveId);
			Assert.IsNotNull(cachedData, "collection was not in cache");

			session.Close();

			session = OpenSession();
			session.EnableFilter("fulfilledOrders").SetParameter("asOfDate", testData.lastMonth);
			sp = (Salesperson) session.CreateQuery("from Salesperson as s where s.id = :id")
					.SetInt64("id", testData.steveId)
					.UniqueResult();
			Assert.AreEqual(1, sp.Orders.Count, "Filtered-collection not bypassing 2L-cache");

			object[] cachedData2 = (object[]) persister.Cache.Cache.Get(testData.steveId);
			Assert.IsNotNull(cachedData2, "collection no longer in cache!");
			Assert.AreSame(cachedData, cachedData2, "Different cache values!");

			session.Close();

			session = OpenSession();
			session.EnableFilter("fulfilledOrders").SetParameter("asOfDate", testData.lastMonth);
			sp = (Salesperson) session.Load(typeof(Salesperson), testData.steveId);
			Assert.AreEqual(1, sp.Orders.Count, "Filtered-collection not bypassing 2L-cache");

			session.Close();

			// Finally, make sure that the original cached version did not get over-written
			session = OpenSession();
			sp = (Salesperson) session.Load(typeof(Salesperson), testData.steveId);
			Assert.AreEqual(2, sp.Orders.Count, "Actual cached version got over-written");

			session.Close();
			testData.Release();
		}

		[Test]
		public void CombinedClassAndCollectionFiltersEnabled()
		{
			TestData testData = new TestData(this);
			testData.Prepare();

			ISession session = OpenSession();
			session.EnableFilter("regionlist").SetParameterList("regions", new string[] { "LA", "APAC" });
			session.EnableFilter("fulfilledOrders").SetParameter("asOfDate", testData.lastMonth);

			// test retreival through hql with the collection as non-eager
			IList salespersons = session.CreateQuery("select s from Salesperson as s").List();
			Assert.AreEqual(1, salespersons.Count, "Incorrect salesperson count");
			Salesperson sp = (Salesperson) salespersons[0];
			Assert.AreEqual(1, sp.Orders.Count, "Incorrect order count");

			session.Clear();

			// test retreival through hql with the collection join fetched
			salespersons = session.CreateQuery("select s from Salesperson as s left join fetch s.orders").List();
			Assert.AreEqual(1, salespersons.Count, "Incorrect salesperson count");
			sp = (Salesperson) salespersons[0];
			Assert.AreEqual(sp.Orders.Count, 1, "Incorrect order count");

			session.Close();
			testData.Release();
		}

		[Test]
		public void HqlFilters()
		{
			//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			// HQL test
			//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			log.Info("Starting HQL filter tests");
			TestData testData = new TestData(this);
			testData.Prepare();

			ISession session = OpenSession();
			session.EnableFilter("region").SetParameter("region", "APAC");

			session.EnableFilter("effectiveDate")
					.SetParameter("asOfDate", testData.lastMonth);

			log.Info("HQL against Salesperson...");
			IList results = session.CreateQuery("select s from Salesperson as s left join fetch s.Orders").List();
			Assert.IsTrue(results.Count == 1, "Incorrect filtered HQL result count [" + results.Count + "]");
			Salesperson result = (Salesperson) results[0];
			Assert.IsTrue(result.Orders.Count == 1, "Incorrect collectionfilter count");

			log.Info("HQL against Product...");
			results = session.CreateQuery("from Product as p where p.StockNumber = ?").SetInt32(0, 124).List();
			Assert.IsTrue(results.Count == 1);

			session.Close();
			testData.Release();
		}
		
		[Test]
		public void CriteriaQueryFilters()
		{
			//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			// Criteria-query test
			//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			log.Info("Starting Criteria-query filter tests");
			TestData testData = new TestData(this);
			testData.Prepare();

			ISession session = OpenSession();
			session.EnableFilter("region").SetParameter("region", "APAC");

			session.EnableFilter("fulfilledOrders")
					.SetParameter("asOfDate", testData.lastMonth);

			session.EnableFilter("effectiveDate")
					.SetParameter("asOfDate", testData.lastMonth);

			log.Info("Criteria query against Salesperson...");
			IList salespersons = session.CreateCriteria(typeof(Salesperson))
					.SetFetchMode("orders", FetchMode.Join)
					.List();
			Assert.AreEqual(1, salespersons.Count, "Incorrect salesperson count");
			Assert.AreEqual(1, ((Salesperson) salespersons[0]).Orders.Count, "Incorrect order count");

			log.Info("Criteria query against Product...");
			IList products = session.CreateCriteria(typeof(Product))
					.Add(Expression.Expression.Eq("stockNumber", 124))
					.List();
			Assert.AreEqual(1, products.Count, "Incorrect product count");

			session.Close();
			testData.Release();
		}

		public void testGetFilters()
		{
			//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			// Get() test
			//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			log.Info("Starting get() filter tests (eager assoc. fetching).");
			TestData testData = new TestData(this);
			testData.Prepare();

			ISession session = OpenSession();
			session.EnableFilter("region").SetParameter("region", "APAC");

			log.Info("Performing get()...");
			Salesperson salesperson = (Salesperson) session.Get(typeof(Salesperson), testData.steveId);
			Assert.IsNotNull(salesperson);
			Assert.AreEqual(1, salesperson.Orders.Count, "Incorrect order count");

			session.Close();
			testData.Release();
		}

		public void testOneToManyFilters()
		{
			//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			// one-to-many loading tests
			//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			log.Info("Starting one-to-many collection loader filter tests.");
			TestData testData = new TestData(this);
			testData.Prepare();

			ISession session = OpenSession();
			session.EnableFilter("seniorSalespersons")
				.SetParameter("asOfDate", testData.lastMonth);

			log.Info("Performing Load of Department...");
			Department department = (Department) session.Load(typeof(Department), testData.deptId);
			ISet salespersons = department.Salespersons;
			Assert.AreEqual(1, salespersons.Count, "Incorrect salesperson count");

			session.Close();
			testData.Release();
		}

		[Test]
		public void InStyleFilterParameter()
		{
			//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			// one-to-many loading tests
			//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			log.Info("Starting one-to-many collection loader filter tests.");
			TestData testData = new TestData(this);
			testData.Prepare();

			ISession session = OpenSession();
			session.EnableFilter("regionlist")
					.SetParameterList("regions", new string[] { "LA", "APAC" });

			log.Debug("Performing query of Salespersons");
			IList salespersons = session.CreateQuery("from Salesperson").List();
			Assert.AreEqual(1, salespersons.Count, "Incorrect salesperson count");

			session.Close();
			testData.Release();
		}

		public void ManyToManyFilterOnCriteria()
		{
			TestData testData = new TestData(this);
			testData.Prepare();

			ISession session = OpenSession();
			session.EnableFilter("effectiveDate").SetParameter("asOfDate", DateTime.Today);

			Product prod = (Product) session.CreateCriteria(typeof(Product))
					.SetResultTransformer(new DistinctRootEntityResultTransformer())
					.Add(Expression.Expression.Eq("id", testData.prod1Id))
					.UniqueResult();

			Assert.IsNotNull(prod);
			Assert.AreEqual(1, prod.Categories.Count, "Incorrect Product.categories count for filter");

			session.Close();
			testData.Release();
		}

		[Test, Ignore("Statistics not implemented")]
		public void ManyToManyFilterOnLoad()
		{
			//TestData testData = new TestData(this);
			//testData.Prepare();

			//ISession session = OpenSession();
			//session.EnableFilter("effectiveDate").SetParameter("asOfDate", DateTime.Today);

			//Product prod = (Product) session.Get(typeof(Product), testData.prod1Id);

			//long initLoadCount = sessions.getStatistics().getCollectionLoadCount();
			//long initFetchCount = sessions.getStatistics().getCollectionFetchCount();

			//// should already have been initialized...
			//int size = prod.Categories.Count;
			//Assert.AreEqual(1, size, "Incorrect filtered collection count");

			//long currLoadCount = sessions.getStatistics().getCollectionLoadCount();
			//long currFetchCount = sessions.getStatistics().getCollectionFetchCount();

			//Assert.IsTrue(
			//    (initLoadCount == currLoadCount) && (initFetchCount == currFetchCount),
			//    "Load with join fetch of many-to-many did not trigger join fetch"

			//    );

			//// make sure we did not get back a collection of proxies
			//long initEntityLoadCount = sessions.getStatistics().getEntityLoadCount();

			//foreach (Category cat in prod.Categories)
			//{
			//    Console.WriteLine(" ===> " + cat.Name);
			//}
			//long currEntityLoadCount = sessions.getStatistics().getEntityLoadCount();

			//Assert.IsTrue(
			//    (initEntityLoadCount == currEntityLoadCount),
			//    "Load with join fetch of many-to-many did not trigger *complete* join fetch"
			//    );

			//session.Close();
			//testData.Release();
		}

		[Test]
		public void ManyToManyOnCollectionLoadAfterHQL()
		{
			TestData testData = new TestData(this);
			testData.Prepare();

			ISession session = OpenSession();
			session.EnableFilter("effectiveDate").SetParameter("asOfDate", DateTime.Today);

			// Force the categories to not get initialized here
			IList result = session.CreateQuery("from Product as p where p.id = :id")
					.SetInt64("id", testData.prod1Id)
					.List();
			Assert.IsTrue(result.Count > 0, "No products returned from HQL");

			Product prod = (Product) result[0];
			Assert.IsNotNull(prod);
			Assert.AreEqual(1, prod.Categories.Count, "Incorrect Product.categories count for filter on collection Load");

			session.Close();
			testData.Release();
		}

		[Test]
		public void ManyToManyFilterOnQuery()
		{
			TestData testData = new TestData(this);
			testData.Prepare();

			ISession session = OpenSession();
			session.EnableFilter("effectiveDate").SetParameter("asOfDate", DateTime.Today);

			IList result = session.CreateQuery("from Product p inner join fetch p.categories").List();
			Assert.IsTrue(result.Count > 0, "No products returned from HQL many-to-many filter case");

			Product prod = (Product) result[0];

			Assert.IsNotNull(prod);
			Assert.AreEqual(1, prod.Categories.Count, "Incorrect Product.categories count for filter with HQL");

			session.Close();
			testData.Release();
		}

		[Test, Ignore("Statistics not implemented")]
		public void ManyToManyBase()
		{
			//TestData testData = new TestData(this);
			//testData.Prepare();

			//ISession session = OpenSession();

			//Product prod = ( Product ) session.Get( typeof(Product), testData.prod1Id );

			//long initLoadCount = sessions.getStatistics().getCollectionLoadCount();
			//long initFetchCount = sessions.getStatistics().getCollectionFetchCount();

			//// should already have been initialized...
			//int size = prod.Categories.Count;
			//Assert.AreEqual(2, size,  "Incorrect non-filtered collection count" );

			//long currLoadCount = sessions.getStatistics().getCollectionLoadCount();
			//long currFetchCount = sessions.getStatistics().getCollectionFetchCount();

			//Assert.IsTrue(
			//        ( initLoadCount == currLoadCount ) && ( initFetchCount == currFetchCount ),
			//        "Load with join fetch of many-to-many did not trigger join fetch"
			//);

			//// make sure we did not get back a collection of proxies
			//long initEntityLoadCount = sessions.getStatistics().getEntityLoadCount();
			//foreach (Category cat in prod.Categories)
			//{
			//    Console.WriteLine(" ===> " + cat.Name);
			//}
			//long currEntityLoadCount = sessions.getStatistics().getEntityLoadCount();

			//Assert.IsTrue(
			//        ( initEntityLoadCount == currEntityLoadCount ),
			//        "Load with join fetch of many-to-many did not trigger *complete* join fetch"
			//);

			//session.Close();
			//testData.Release();
		}

		[Test, Ignore("Statistics not implemented")]
		public void ManyToManyBaseThruCriteria()
		{
			//TestData testData = new TestData(this);
			//testData.Prepare();

			//ISession session = OpenSession();

			//IList result = session.CreateCriteria(typeof(Product))
			//    .Add(Expression.Expression.Eq("id", testData.prod1Id))
			//    .List();

			//Product prod = (Product) result[0];

			//long initLoadCount = sessions.getStatistics().getCollectionLoadCount();
			//long initFetchCount = sessions.getStatistics().getCollectionFetchCount();

			//// should already have been initialized...
			//int size = prod.Categories.Count;
			//Assert.AreEqual(2, size, "Incorrect non-filtered collection count");

			//long currLoadCount = sessions.getStatistics().getCollectionLoadCount();
			//long currFetchCount = sessions.getStatistics().getCollectionFetchCount();

			//Assert.IsTrue(
			//    (initLoadCount == currLoadCount) && (initFetchCount == currFetchCount),
			//    "Load with join fetch of many-to-many did not trigger join fetch"
			//    );

			//// make sure we did not get back a collection of proxies
			//long initEntityLoadCount = sessions.getStatistics().getEntityLoadCount();
			//foreach (Category cat in prod.Categories)
			//{
			//    Console.WriteLine(" ===> " + cat.Name);
			//}
			//long currEntityLoadCount = sessions.getStatistics().getEntityLoadCount();

			//Assert.IsTrue(
			//    (initEntityLoadCount == currEntityLoadCount),
			//    "Load with join fetch of many-to-many did not trigger *complete* join fetch"
			//    );

			//session.Close();
			//testData.Release();
		}

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
						"FilterTest.defs.hbm.xml",
						"FilterTest.classes.hbm.xml",
					};
			}
		}

		protected override void Configure(Configuration cfg)
		{
			cfg.SetProperty(Environment.MaxFetchDepth, "1");
			//		cfg.setProperty( Environment.MAX_FETCH_DEPTH, "2" );
			//cfg.SetProperty(Environment.GenerateStatistics, "true");
			cfg.SetProperty(Environment.UseQueryCache, "true");
			
			foreach (PersistentClass clazz in cfg.ClassMappings)
			{
				if (!clazz.IsInherited)
				{
					cfg.SetCacheConcurrencyStrategy(clazz.MappedClass, new NonstrictReadWriteCache());
				}
			}
			
			foreach (Mapping.Collection coll in cfg.CollectionMappings)
			{
				cfg.SetCacheConcurrencyStrategy(coll.Role, new NonstrictReadWriteCache());
			}
		}

		private class TestData
		{
			public long steveId;
			public long deptId;
			public long prod1Id;
			public DateTime lastMonth;
			public DateTime nextMonth;
			public DateTime sixMonthsAgo;
			public DateTime fourMonthsAgo;

			private DynamicFilterTest outer;
			
			public TestData(DynamicFilterTest outer)
			{
				this.outer = outer;
			}

			public IList entitiesToCleanUp = new ArrayList();

			public void Prepare()
			{
				ISession session = outer.OpenSession();
				ITransaction transaction = session.BeginTransaction();

				lastMonth = DateTime.Today.AddMonths(-1);
				nextMonth = DateTime.Today.AddMonths(1);

				sixMonthsAgo = DateTime.Today.AddMonths(-6);
				fourMonthsAgo = DateTime.Today.AddMonths(-4);

				Department dept = new Department();
				dept.Name = ("Sales");

				session.Save(dept);
				deptId = dept.Id;
				entitiesToCleanUp.Add(dept);

				Salesperson steve = new Salesperson();
				steve.Name = ("steve");
				steve.Region = ("APAC");
				steve.HireDate = (sixMonthsAgo);

				steve.Department = (dept);
				dept.Salespersons.Add(steve);

				Salesperson max = new Salesperson();
				max.Name = ("max");
				max.Region = ("EMEA");
				max.HireDate = (nextMonth);

				max.Department = (dept);
				dept.Salespersons.Add(max);

				session.Save(steve);
				session.Save(max);
				entitiesToCleanUp.Add(steve);
				entitiesToCleanUp.Add(max);

				steveId = steve.Id;

				Category cat1 = new Category("test cat 1", lastMonth, nextMonth);
				Category cat2 = new Category("test cat 2", sixMonthsAgo, fourMonthsAgo);

				Product product1 = new Product();
				product1.Name = ("Acme Hair Gel");
				product1.StockNumber = (123);
				product1.EffectiveStartDate = (lastMonth);
				product1.EffectiveEndDate = (nextMonth);

				product1.AddCategory(cat1);
				product1.AddCategory(cat2);

				session.Save(product1);
				entitiesToCleanUp.Add(product1);
				prod1Id = product1.Id;

				Order order1 = new Order();
				order1.Buyer = "gavin";
				order1.Region = ("APAC");
				order1.PlacementDate = sixMonthsAgo;
				order1.FulfillmentDate = fourMonthsAgo;
				order1.Salesperson = steve;
				order1.AddLineItem(product1, 500);

				session.Save(order1);
				entitiesToCleanUp.Add(order1);

				Product product2 = new Product();
				product2.Name = ("Acme Super-Duper DTO Factory");
				product2.StockNumber = (124);
				product2.EffectiveStartDate = (sixMonthsAgo);
				product2.EffectiveEndDate = (DateTime.Today);

				Category cat3 = new Category("test cat 2", sixMonthsAgo, DateTime.Today);
				product2.AddCategory(cat3);

				session.Save(product2);
				entitiesToCleanUp.Add(product2);

				// An uncategorized product
				Product product3 = new Product();
				product3.Name = ("Uncategorized product");
				session.Save(product3);
				entitiesToCleanUp.Add(product3);

				Order order2 = new Order();
				order2.Buyer = "christian";
				order2.Region = ("EMEA");
				order2.PlacementDate = lastMonth;
				order2.Salesperson = steve;
				order2.AddLineItem(product2, -1);

				session.Save(order2);
				entitiesToCleanUp.Add(order2);

				transaction.Commit();
				session.Close();
			}

			public void Release()
			{
				ISession session = outer.OpenSession();
				ITransaction transaction = session.BeginTransaction();

				foreach (object obj in entitiesToCleanUp)
				{
					session.Delete(obj);
				}

				transaction.Commit();
				session.Close();
			}
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3848
{
	[TestFixture]
	public abstract class Fixture : TestCaseMappingByCode
	{
		protected Customer Customer1;
		protected Customer Customer2;
		protected Customer Customer3;
		protected const int OrderNumber = 2;

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Customer>(rc =>
			{
				rc.Table("Customers");
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.Set(x => x.Orders, m =>
				{
					m.Inverse(true);
					m.Key(k =>
					{
						k.Column("CustomerId");
						k.NotNullable(true);
					});
					m.Cascade(Mapping.ByCode.Cascade.All.Include(Mapping.ByCode.Cascade.DeleteOrphans));
					m.Cache(c => c.Usage(CacheUsage.ReadWrite));
				}, m => m.OneToMany());

				rc.Set(x => x.Companies, m =>
				{
					m.Inverse(true);
					m.Key(k =>
					{
						k.Column("CustomerId");
						k.NotNullable(true);
					});
					m.Cascade(Mapping.ByCode.Cascade.All.Include(Mapping.ByCode.Cascade.DeleteOrphans));
					m.Cache(c => c.Usage(CacheUsage.ReadWrite));
				}, m => m.OneToMany());
			});

			mapper.Class<Order>(rc =>
			{
				rc.Table("Orders");
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Number, m => m.Column("`Number`"));
				rc.ManyToOne(x => x.Customer, m => m.Column("CustomerId"));
				rc.Cache(c => c.Usage(CacheUsage.ReadWrite));
			});

			mapper.Class<Company>(rc =>
			{
				rc.Table("Companies");
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.ManyToOne(x => x.Customer, m => m.Column("CustomerId"));
				rc.Cache(c => c.Usage(CacheUsage.ReadWrite));
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			configuration.Cache(c =>
			{
				c.UseQueryCache = true;
				c.Provider<HashtableCacheProvider>();
			});
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				Customer1 = new Customer { Name = "First Customer" };

				Customer2 = new Customer { Name = "Second Customer" };

				Customer3 = new Customer { Name = "Third Customer" };

				var customer1Order1 = new Order { Number = 1 };
				var customer1Company1 = new Company { Name = "First" };
				Customer1.AddOrder(customer1Order1);
				Customer1.AddCompany(customer1Company1);

				var customer1Order2 = new Order { Number = 2 };
				var customer1Company2 = new Company { Name = "Second" };
				Customer1.AddOrder(customer1Order2);
				Customer1.AddCompany(customer1Company2);

				var customer2Order1 = new Order { Number = 1 };
				var customer2Company1 = new Company { Name = "First" };
				Customer2.AddOrder(customer2Order1);
				Customer2.AddCompany(customer2Company1);

				var customer2Order2 = new Order { Number = 2 };
				var customer2Company2 = new Company { Name = "Second" };
				Customer2.AddOrder(customer2Order2);
				Customer2.AddCompany(customer2Company2);

				var customer3Company1 = new Company { Name = "First" };
				var customer3Order1 = new Order { Number = 1 };
				Customer3.AddOrder(customer3Order1);
				Customer3.AddCompany(customer3Company1);

				session.Save(Customer1);
				session.Save(Customer2);
				session.Save(Customer3);

				transaction.Commit();
				session.Flush();
			}
		}

		protected override void OnTearDown()
		{
			ClearSecondLevelCacheFor(typeof(Customer));
			ClearCollectionCache<Customer>(n => n.Orders);
			ClearCollectionCache<Customer>(n => n.Companies);
			ClearSecondLevelCacheFor(typeof(Order));

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public virtual void ChildCollectionsFromLeftOuterJoinWithOnClauseRestrictionOnCollectionShouldNotBeInSecondLevelCache()
		{
			using (var firstSession = OpenSession())
			{
				IList<Customer> customersWithOrderNumberEqualsTo2;
				using (var tx = firstSession.BeginTransaction())
				{
					customersWithOrderNumberEqualsTo2 =
						GetCustomersByOrderNumberUsingOnClause(firstSession, OrderNumber);
					tx.Commit();
				}

				using (var secondSession = OpenSession())
				using (var tx = secondSession.BeginTransaction())
				{
					var customers = GetAllCustomers(secondSession);

					Assert.That(
						customersWithOrderNumberEqualsTo2.Single(n => n.Id == Customer1.Id).Orders,
						Has.Count.EqualTo(Customer1.Orders.Count(n => n.Number == OrderNumber)));
					Assert.That(
						customersWithOrderNumberEqualsTo2.Single(n => n.Id == Customer2.Id).Orders,
						Has.Count.EqualTo(Customer2.Orders.Count(n => n.Number == OrderNumber)));
					Assert.That(
						customersWithOrderNumberEqualsTo2.Single(n => n.Id == Customer3.Id).Orders,
						Has.Count.EqualTo(Customer3.Orders.Count(n => n.Number == OrderNumber)));

					Assert.That(
						customers.Single(n => n.Id == Customer1.Id).Orders,
						Has.Count.EqualTo(Customer1.Orders.Count));
					Assert.That(
						customers.Single(n => n.Id == Customer2.Id).Orders,
						Has.Count.EqualTo(Customer2.Orders.Count));
					Assert.That(
						customers.Single(n => n.Id == Customer3.Id).Orders,
						Has.Count.EqualTo(Customer3.Orders.Count));

					tx.Commit();
				}
			}
		}

		[Test]
		public virtual void ChildCollectionsWithSelectModeFetchOnCollectionShouldNotBeInSecondLevelCache()
		{
			using (var firstSession = OpenSession())
			{
				IList<Customer> customersWithOrderNumberEqualsTo2;
				using (var tx = firstSession.BeginTransaction())
				{
					customersWithOrderNumberEqualsTo2 = GetCustomersByOrderNumberUsingFetch(firstSession, OrderNumber);
					tx.Commit();
				}

				using (var secondSession = OpenSession())
				using (var tx = secondSession.BeginTransaction())
				{
					var customers = GetAllCustomers(secondSession);

					Assert.That(
						customersWithOrderNumberEqualsTo2.Single(n => n.Id == Customer1.Id).Orders,
						Has.Count.EqualTo(Customer1.Orders.Count(n => n.Number == OrderNumber)));
					Assert.That(
						customersWithOrderNumberEqualsTo2.Single(n => n.Id == Customer2.Id).Orders,
						Has.Count.EqualTo(Customer2.Orders.Count(n => n.Number == OrderNumber)));
					Assert.That(
						customersWithOrderNumberEqualsTo2,
						Has.Count.EqualTo(2));

					Assert.That(
						customers.Single(n => n.Id == Customer1.Id).Orders,
						Has.Count.EqualTo(Customer1.Orders.Count));
					Assert.That(
						customers.Single(n => n.Id == Customer2.Id).Orders,
						Has.Count.EqualTo(Customer2.Orders.Count));
					Assert.That(
						customers.Single(n => n.Id == Customer3.Id).Orders,
						Has.Count.EqualTo(Customer3.Orders.Count));

					tx.Commit();
				}
			}
		}

		[Test]
		public virtual void ChildCollectionsWithSelectModeFetchAndWhereClauseShouldNotBeInSecondLevelCache()
		{
			using (var firstSession = OpenSession())
			{
				IList<Customer> customersWithOrderNumberEqualsTo2;
				using (var tx = firstSession.BeginTransaction())
				{
					customersWithOrderNumberEqualsTo2 =
						GetCustomersByOrderNumberUsingFetchAndWhereClause(firstSession, OrderNumber);
					tx.Commit();
				}

				using (var secondSession = OpenSession())
				using (var tx = secondSession.BeginTransaction())
				{
					var customers = GetAllCustomers(secondSession);

					Assert.That(
						customersWithOrderNumberEqualsTo2.Single(n => n.Id == Customer1.Id).Orders,
						Has.Count.EqualTo(Customer1.Orders.Count(n => n.Number == OrderNumber)));
					Assert.That(
						customersWithOrderNumberEqualsTo2.Single(n => n.Id == Customer2.Id).Orders,
						Has.Count.EqualTo(Customer2.Orders.Count(n => n.Number == OrderNumber)));
					Assert.That(
						customersWithOrderNumberEqualsTo2,
						Has.Count.EqualTo(2));

					Assert.That(
						customers.Single(n => n.Id == Customer1.Id).Orders,
						Has.Count.EqualTo(Customer1.Orders.Count));
					Assert.That(
						customers.Single(n => n.Id == Customer2.Id).Orders,
						Has.Count.EqualTo(Customer2.Orders.Count));
					Assert.That(
						customers.Single(n => n.Id == Customer3.Id).Orders,
						Has.Count.EqualTo(Customer3.Orders.Count));

					tx.Commit();
				}
			}
		}

		[Test]
		public void ChildCollectionsFromLeftOuterJoinWithWhereClauseRestrictionOnCollectionShouldNotBeInSecondLevelCache()
		{
			using (var firstSession = OpenSession())
			{
				IList<Customer> customersWithOrderNumberEqualsTo2;
				using (var tx = firstSession.BeginTransaction())
				{
					customersWithOrderNumberEqualsTo2 =
						GetCustomersByOrderNumberUsingWhereClause(firstSession, OrderNumber);
					tx.Commit();
				}

				using (var secondSession = OpenSession())
				using (var tx = secondSession.BeginTransaction())
				{
					var customers = GetAllCustomers(secondSession);

					Assert.That(
						customersWithOrderNumberEqualsTo2.Single(n => n.Id == Customer1.Id).Orders,
						Has.Count.EqualTo(Customer1.Orders.Count(n => n.Number == OrderNumber)));
					Assert.That(
						customersWithOrderNumberEqualsTo2.Single(n => n.Id == Customer2.Id).Orders,
						Has.Count.EqualTo(Customer2.Orders.Count(n => n.Number == OrderNumber)));

					Assert.That(
						customers.Single(n => n.Id == Customer3.Id).Orders,
						Has.Count.EqualTo(Customer3.Orders.Count));
					Assert.That(
						customers.Single(n => n.Id == Customer1.Id).Orders,
						Has.Count.EqualTo(Customer1.Orders.Count));
					Assert.That(
						customers.Single(n => n.Id == Customer2.Id).Orders,
						Has.Count.EqualTo(Customer2.Orders.Count));

					tx.Commit();
				}
			}
		}

		[Test]
		public void ChildCollectionsEagerFetchedShouldBeInSecondLevelCache()
		{
			using (var firstSession = OpenSession())
			{
				IList<Customer> customersWithOrderNumberEqualsTo2;
				using (var tx = firstSession.BeginTransaction())
				{
					customersWithOrderNumberEqualsTo2 = GetCustomersWithOrdersEagerLoaded(firstSession);
					tx.Commit();
				}

				using (var session = OpenSession())
				using (IDbCommand cmd = session.Connection.CreateCommand())
				{
					cmd.CommandText = "DELETE FROM Orders";
					cmd.ExecuteNonQuery();
				}

				using (var secondSession = OpenSession())
				using (var tx = secondSession.BeginTransaction())
				{
					var customers = GetAllCustomers(secondSession);

					Assert.That(
						customersWithOrderNumberEqualsTo2.Single(n => n.Id == Customer1.Id).Orders,
						Has.Count.EqualTo(Customer1.Orders.Count));
					Assert.That(
						customersWithOrderNumberEqualsTo2.Single(n => n.Id == Customer2.Id).Orders,
						Has.Count.EqualTo(Customer2.Orders.Count));

					Assert.That(
						customers.Single(n => n.Id == Customer3.Id).Orders,
						Has.Count.EqualTo(Customer3.Orders.Count));
					Assert.That(
						customers.Single(n => n.Id == Customer1.Id).Orders,
						Has.Count.EqualTo(Customer1.Orders.Count));
					Assert.That(
						customers.Single(n => n.Id == Customer2.Id).Orders,
						Has.Count.EqualTo(Customer2.Orders.Count));

					tx.Commit();
				}
			}
		}

		[Test]
		public void ChildCollectionsFromLeftOuterJoinWithWhereClauseRestrictionOnRootShouldBeInSecondLevelCache()
		{
			using (var firstSession = OpenSession())
			{
				IList<Customer> customersWithOrderNumberEqualsTo2;
				using (var tx = firstSession.BeginTransaction())
				{
					customersWithOrderNumberEqualsTo2 =
						GetCustomersByNameUsingWhereClause(firstSession, "First Customer");
					tx.Commit();
				}

				using (var session = OpenSession())
				using (IDbCommand cmd = session.Connection.CreateCommand())
				{
					cmd.CommandText = "DELETE FROM Orders";
					cmd.ExecuteNonQuery();
				}

				using (var secondSession = OpenSession())
				using (var tx = secondSession.BeginTransaction())
				{
					var customers = secondSession.Get<Customer>(Customer1.Id);

					Assert.That(
						customersWithOrderNumberEqualsTo2.Single(n => n.Id == Customer1.Id).Orders,
						Has.Count.EqualTo(Customer1.Orders.Count));
					Assert.That(customers.Orders, Has.Count.EqualTo(Customer1.Orders.Count));

					tx.Commit();
				}
			}
		}

		[Test]
		public void ChildCollectionsFromLeftOuterJoinShouldBeInSecondLevelCacheIfQueryContainsSubqueryWithRestrictionOnLeftOuterJoin()
		{
			using (var firstSession = OpenSession())
			{
				IList<Customer> customersWithOrderNumberEqualsTo2;
				using (var tx = firstSession.BeginTransaction())
				{
					customersWithOrderNumberEqualsTo2 =
						GetCustomersByOrderNumberUsingSubqueriesAndByNameUsingWhereClause(
							firstSession,
							OrderNumber,
							Customer1.Name);
					tx.Commit();
				}

				using (var secondSession = OpenSession())
				{
					IList<Customer> customers;
					using (var tx = secondSession.BeginTransaction())
					{
						customers = GetAllCustomers(secondSession);

						Assert.That(
							customersWithOrderNumberEqualsTo2.Single(n => n.Id == Customer1.Id).Orders,
							Has.Count.EqualTo(Customer1.Orders.Count));

						tx.Commit();
					}

					using (var thirdSession = OpenSession())
					using (IDbCommand cmd = thirdSession.Connection.CreateCommand())
					{
						cmd.CommandText = "DELETE FROM Orders";
						cmd.ExecuteNonQuery();
					}

					Assert.That(
						customers.Single(n => n.Id == Customer1.Id).Orders,
						Has.Count.EqualTo(Customer1.Orders.Count));
					Assert.That(customers.Single(n => n.Id == Customer2.Id).Orders, Has.Count.EqualTo(0));
					Assert.That(customers.Single(n => n.Id == Customer3.Id).Orders, Has.Count.EqualTo(0));
				}
			}
		}

		[Test]
		public virtual void ChildCollectionsFromLeftOuterJoinOnlyWithRestrictionShouldNotBeIn2LvlCache()
		{
			using (var firstSession = OpenSession())
			{
				IList<Customer> customersWithOrderNumberEqualsTo2AndCompanies;
				using (var tx = firstSession.BeginTransaction())
				{
					customersWithOrderNumberEqualsTo2AndCompanies =
						GetCustomersWithCompaniesByOrderNumberUsingOnClause(firstSession, OrderNumber);
					tx.Commit();
				}

				using (var session = OpenSession())
				using (IDbCommand cmd = session.Connection.CreateCommand())
				{
					cmd.CommandText = "DELETE FROM Orders";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "DELETE FROM Companies";
					cmd.ExecuteNonQuery();
				}

				using (var secondSession = OpenSession())
				using (var tx = secondSession.BeginTransaction())
				{
					var customers = GetAllCustomers(secondSession);

					Assert.That(
						customersWithOrderNumberEqualsTo2AndCompanies.First(n => n.Id == Customer1.Id).Orders,
						Has.Count.EqualTo(Customer1.Orders.Count(n => n.Number == OrderNumber)));
					Assert.That(
						customersWithOrderNumberEqualsTo2AndCompanies.First(n => n.Id == Customer2.Id).Orders,
						Has.Count.EqualTo(Customer2.Orders.Count(n => n.Number == OrderNumber)));
					Assert.That(
						customersWithOrderNumberEqualsTo2AndCompanies.First(n => n.Id == Customer3.Id).Orders,
						Has.Count.EqualTo(Customer3.Orders.Count(n => n.Number == OrderNumber)));

					Assert.That(
						customersWithOrderNumberEqualsTo2AndCompanies.First(n => n.Id == Customer1.Id).Companies,
						Has.Count.EqualTo(Customer1.Companies.Count));
					Assert.That(
						customersWithOrderNumberEqualsTo2AndCompanies.First(n => n.Id == Customer2.Id).Companies,
						Has.Count.EqualTo(Customer2.Companies.Count));
					Assert.That(
						customersWithOrderNumberEqualsTo2AndCompanies.First(n => n.Id == Customer3.Id).Companies,
						Has.Count.EqualTo(Customer3.Companies.Count));

					Assert.That(customers.Single(n => n.Id == Customer1.Id).Orders, Has.Count.EqualTo(0));
					Assert.That(customers.Single(n => n.Id == Customer2.Id).Orders, Has.Count.EqualTo(0));
					Assert.That(customers.Single(n => n.Id == Customer3.Id).Orders, Has.Count.EqualTo(0));

					Assert.That(
						customers.Single(n => n.Id == Customer1.Id).Companies,
						Has.Count.EqualTo(Customer1.Companies.Count));
					Assert.That(
						customers.Single(n => n.Id == Customer2.Id).Companies,
						Has.Count.EqualTo(Customer2.Companies.Count));
					Assert.That(
						customers.Single(n => n.Id == Customer3.Id).Companies,
						Has.Count.EqualTo(Customer3.Companies.Count));

					tx.Commit();
				}
			}
		}

		[Test]
		public virtual void ChildCollectionsWithoutRestrictionShouldBeIn2LvlCache()
		{
			using (var firstSession = OpenSession())
			{
				IList<Customer> customersWithOrdersAndCompaniesWithoutRestrictions;
				using (var tx = firstSession.BeginTransaction())
				{
					customersWithOrdersAndCompaniesWithoutRestrictions =
						GetCustomersAndCompaniesByOrderNumberUsingFetchWithoutRestrictions(firstSession);
					tx.Commit();
				}

				using (var session = OpenSession())
				using (IDbCommand cmd = session.Connection.CreateCommand())
				{
					cmd.CommandText = "DELETE FROM Orders";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "DELETE FROM Companies";
					cmd.ExecuteNonQuery();
				}

				using (var secondSession = OpenSession())
				using (var tx = secondSession.BeginTransaction())
				{
					var customers = GetAllCustomers(secondSession);

					Assert.That(
						customersWithOrdersAndCompaniesWithoutRestrictions.First(n => n.Id == Customer1.Id).Orders,
						Has.Count.EqualTo(Customer1.Orders.Count()));
					Assert.That(
						customersWithOrdersAndCompaniesWithoutRestrictions.First(n => n.Id == Customer2.Id).Orders,
						Has.Count.EqualTo(Customer2.Orders.Count()));
					Assert.That(
						customersWithOrdersAndCompaniesWithoutRestrictions.First(n => n.Id == Customer3.Id).Orders,
						Has.Count.EqualTo(Customer3.Orders.Count()));

					Assert.That(
						customersWithOrdersAndCompaniesWithoutRestrictions.First(n => n.Id == Customer1.Id).Companies,
						Has.Count.EqualTo(Customer1.Companies.Count));
					Assert.That(
						customersWithOrdersAndCompaniesWithoutRestrictions.First(n => n.Id == Customer2.Id).Companies,
						Has.Count.EqualTo(Customer2.Companies.Count));
					Assert.That(
						customersWithOrdersAndCompaniesWithoutRestrictions.First(n => n.Id == Customer3.Id).Companies,
						Has.Count.EqualTo(Customer3.Companies.Count));

					Assert.That(
						customers.First(n => n.Id == Customer1.Id).Orders,
						Has.Count.EqualTo(Customer1.Orders.Count()));
					Assert.That(
						customers.First(n => n.Id == Customer2.Id).Orders,
						Has.Count.EqualTo(Customer2.Orders.Count()));
					Assert.That(
						customers.First(n => n.Id == Customer3.Id).Orders,
						Has.Count.EqualTo(Customer3.Orders.Count()));

					Assert.That(
						customers.First(n => n.Id == Customer1.Id).Companies,
						Has.Count.EqualTo(Customer1.Companies.Count()));
					Assert.That(
						customers.First(n => n.Id == Customer2.Id).Companies,
						Has.Count.EqualTo(Customer2.Companies.Count()));
					Assert.That(
						customers.First(n => n.Id == Customer3.Id).Companies,
						Has.Count.EqualTo(Customer3.Companies.Count()));

					tx.Commit();
				}
			}
		}

		[Test]
		public virtual void ChildCollectionsWithRestrictionShouldNotBeIn2LvlCache()
		{
			using (var firstSession = OpenSession())
			{
				IList<Customer> customersWithOrdersAndCompaniesWithRestrictions;
				using (var tx = firstSession.BeginTransaction())
				{
					customersWithOrdersAndCompaniesWithRestrictions =
						GetCustomersAndCompaniesByOrderNumberUsingFetchAndWhereClause(
							firstSession,
							OrderNumber,
							"Second");
					tx.Commit();
				}

				using (var session = OpenSession())
				using (IDbCommand cmd = session.Connection.CreateCommand())
				{
					cmd.CommandText = "DELETE FROM Orders";
					cmd.ExecuteNonQuery();
					cmd.CommandText = "DELETE FROM Companies";
					cmd.ExecuteNonQuery();
				}

				using (var secondSession = OpenSession())
				using (var tx = secondSession.BeginTransaction())
				{
					var customers = GetAllCustomers(secondSession);

					Assert.That(
						customersWithOrdersAndCompaniesWithRestrictions.First(n => n.Id == Customer1.Id).Orders,
						Has.Count.EqualTo(Customer1.Orders.Count(n => n.Number == OrderNumber)));
					Assert.That(
						customersWithOrdersAndCompaniesWithRestrictions.First(n => n.Id == Customer2.Id).Orders,
						Has.Count.EqualTo(Customer2.Orders.Count(n => n.Number == OrderNumber)));

					Assert.That(
						customersWithOrdersAndCompaniesWithRestrictions.First(n => n.Id == Customer1.Id).Companies,
						Has.Count.EqualTo(Customer1.Companies.Count(n => n.Name == "Second")));
					Assert.That(
						customersWithOrdersAndCompaniesWithRestrictions.First(n => n.Id == Customer2.Id).Companies,
						Has.Count.EqualTo(Customer2.Companies.Count(n => n.Name == "Second")));

					Assert.That(customers.Single(n => n.Id == Customer1.Id).Orders, Has.Count.EqualTo(0));
					Assert.That(customers.Single(n => n.Id == Customer2.Id).Orders, Has.Count.EqualTo(0));
					Assert.That(customers.Single(n => n.Id == Customer3.Id).Orders, Has.Count.EqualTo(0));

					Assert.That(
						customers.Single(n => n.Id == Customer1.Id).Companies,
						Has.Count.EqualTo(0));
					Assert.That(
						customers.Single(n => n.Id == Customer2.Id).Companies,
						Has.Count.EqualTo(0));
					Assert.That(
						customers.Single(n => n.Id == Customer3.Id).Companies,
						Has.Count.EqualTo(0));

					tx.Commit();
				}
			}
		}

		protected void ClearSecondLevelCacheFor(System.Type entity)
		{
			var entityName = entity.FullName;
			Sfi.EvictEntity(entityName);
			var entityPersister = Sfi.GetEntityPersister(entityName);
			if (!entityPersister.HasCache)
				return;

			var querySpaces = entityPersister.QuerySpaces.ToList().AsReadOnly();
			Sfi.UpdateTimestampsCache.PreInvalidate(querySpaces);
		}

		protected void ClearCollectionCache<T>(Expression<Func<T, IEnumerable>> pathToCollection)
		{
			var rootEntityTypeFullPath = typeof(T).FullName;
			var memberExpression = pathToCollection.Body as MemberExpression;
			if (memberExpression == null)
				throw new ArgumentException("pathToCollection should be member expression");

			var role = $"{rootEntityTypeFullPath}.{memberExpression.Member.Name}";
			Sfi.EvictCollection(role);
		}

		protected abstract IList<Customer> GetCustomersWithOrdersEagerLoaded(ISession session);
		protected abstract IList<Customer> GetCustomersByOrderNumberUsingOnClause(ISession session, int orderNumber);
		protected abstract IList<Customer> GetCustomersByOrderNumberUsingWhereClause(ISession session, int orderNumber);
		protected abstract IList<Customer> GetCustomersByNameUsingWhereClause(ISession session, string customerName);
		protected abstract IList<Customer> GetCustomersByOrderNumberUsingFetch(ISession session, int orderNumber);
		protected abstract IList<Customer> GetCustomersByOrderNumberUsingFetchAndWhereClause(ISession session, int orderNumber);
		protected abstract IList<Customer> GetCustomersAndCompaniesByOrderNumberUsingFetchAndWhereClause(ISession session, int orderNumber, string name);
		protected abstract IList<Customer> GetCustomersAndCompaniesByOrderNumberUsingFetchWithoutRestrictions(ISession session);
		protected abstract IList<Customer> GetCustomersByOrderNumberUsingSubqueriesAndByNameUsingWhereClause(
			ISession session,
			int orderNumber,
			string customerName);
		protected abstract IList<Customer> GetCustomersWithCompaniesByOrderNumberUsingOnClause(
			ISession session,
			int orderNumber);
		protected abstract IList<Customer> GetAllCustomers(ISession session);
	}
}

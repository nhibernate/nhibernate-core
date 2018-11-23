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
	public abstract class TestFixture : TestCaseMappingByCode
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
			});

			mapper.Class<Order>(rc =>
			{
				rc.Table("Orders");
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Number);
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
				Customer1.AddOrder(customer1Order1);

				var customer1Order2 = new Order { Number = 2 };
				Customer1.AddOrder(customer1Order2);

				var customer2Order1 = new Order { Number = 1 };
				Customer2.AddOrder(customer2Order1);

				var customer2Order2 = new Order { Number = 2 };
				Customer2.AddOrder(customer2Order2);

				var customer3Order1 = new Order { Number = 1 };
				Customer3.AddOrder(customer3Order1);

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
			var firstSession = OpenSession();
			var customersWithOrderNumberEqualsTo2 = GetCustomersByOrderNumberUsingOnClause(firstSession, OrderNumber);

			var secondSession = OpenSession();
			var customers = GetAllCustomers(secondSession);

			Assert.That(customersWithOrderNumberEqualsTo2.Single(n => n.Id == Customer1.Id).Orders, Has.Count.EqualTo(Customer1.Orders.Count(n => n.Number == OrderNumber)));
			Assert.That(customersWithOrderNumberEqualsTo2.Single(n => n.Id == Customer2.Id).Orders, Has.Count.EqualTo(Customer2.Orders.Count(n => n.Number == OrderNumber)));
			Assert.That(customersWithOrderNumberEqualsTo2.Single(n => n.Id == Customer3.Id).Orders, Has.Count.EqualTo(Customer3.Orders.Count(n => n.Number == OrderNumber)));

			Assert.That(customers.Single(n => n.Id == Customer1.Id).Orders, Has.Count.EqualTo(Customer1.Orders.Count));
			Assert.That(customers.Single(n => n.Id == Customer2.Id).Orders, Has.Count.EqualTo(Customer2.Orders.Count));
			Assert.That(customers.Single(n => n.Id == Customer3.Id).Orders, Has.Count.EqualTo(Customer3.Orders.Count));

			firstSession.Dispose();
			secondSession.Dispose();
		}

		[Test]
		public void ChildCollectionsFromLeftOuterJoinWithWhereClauseRestrictionOnCollectionShouldBeInSecondLevelCache()
		{
			var firstSession = OpenSession();
			var customersWithOrderNumberEqualsTo2 = GetCustomersByOrderNumberUsingWhereClause(firstSession, OrderNumber);

			var secondSession = OpenSession();
			var customers = GetAllCustomers(secondSession);

			Assert.That(customersWithOrderNumberEqualsTo2.Single(n => n.Id == Customer1.Id).Orders, Has.Count.EqualTo(Customer1.Orders.Count(n => n.Number == OrderNumber)));
			Assert.That(customersWithOrderNumberEqualsTo2.Single(n => n.Id == Customer2.Id).Orders, Has.Count.EqualTo(Customer2.Orders.Count(n => n.Number == OrderNumber)));

			Assert.That(customers.Single(n => n.Id == Customer3.Id).Orders, Has.Count.EqualTo(Customer3.Orders.Count));
			Assert.That(customers.Single(n => n.Id == Customer1.Id).Orders, Has.Count.EqualTo(Customer1.Orders.Count));
			Assert.That(customers.Single(n => n.Id == Customer2.Id).Orders, Has.Count.EqualTo(Customer2.Orders.Count));

			firstSession.Dispose();
			secondSession.Dispose();
		}

		[Test]
		public void ChildCollectionsEagerFetchedShouldBeInSecondLevelCache()
		{
			var firstSession = OpenSession();
			var customersWithOrderNumberEqualsTo2 = GetCustomersWithOrdersEagerLoaded(firstSession);

			using (IDbCommand cmd = OpenSession().Connection.CreateCommand())
			{
				cmd.CommandText = "DELETE FROM Orders;";
				cmd.ExecuteNonQuery();
				cmd.Connection.Close();
			}

			var secondSession = OpenSession();
			var customers = GetAllCustomers(secondSession);


			Assert.That(customersWithOrderNumberEqualsTo2.Single(n => n.Id == Customer1.Id).Orders, Has.Count.EqualTo(Customer1.Orders.Count));
			Assert.That(customersWithOrderNumberEqualsTo2.Single(n => n.Id == Customer2.Id).Orders, Has.Count.EqualTo(Customer2.Orders.Count));

			Assert.That(customers.Single(n => n.Id == Customer3.Id).Orders, Has.Count.EqualTo(Customer3.Orders.Count));
			Assert.That(customers.Single(n => n.Id == Customer1.Id).Orders, Has.Count.EqualTo(Customer1.Orders.Count));
			Assert.That(customers.Single(n => n.Id == Customer2.Id).Orders, Has.Count.EqualTo(Customer2.Orders.Count));

			firstSession.Dispose();
			secondSession.Dispose();
		}

		[Test]
		public void ChildCollectionsFromLeftOuterJoinWithWhereClauseRestrictionOnRootShouldBeInSecondLevelCache()
		{
			var firstSession = OpenSession();
			var customersWithOrderNumberEqualsTo2 = GetCustomersByNameUsingWhereClause(firstSession, "First Customer");


			using (IDbCommand cmd = OpenSession().Connection.CreateCommand())
			{
				cmd.CommandText = "DELETE FROM Orders;";
				cmd.ExecuteNonQuery();
				cmd.Connection.Close();
			}

			var secondSession = OpenSession();
			var customers = secondSession.Get<Customer>(Customer1.Id);

			Assert.That(customersWithOrderNumberEqualsTo2.Single(n => n.Id == Customer1.Id).Orders, Has.Count.EqualTo(Customer1.Orders.Count));
			Assert.That(customers.Orders, Has.Count.EqualTo(Customer1.Orders.Count));

			firstSession.Dispose();
			secondSession.Dispose();
		}

		[Test]
		public void ChildCollectionsFromLeftOuterJoinShouldBeInSecondLevelCacheIfQueryContainsSubqueryWithRestrictionOnLeftOuterJoin()
		{
			var firstSession = OpenSession();
			var customersWithOrderNumberEqualsTo2 = GetCustomersByOrderNumberUsingSubqueriesAndByNameUsingWhereClause(firstSession, OrderNumber, Customer1.Name);

			var secondSession = OpenSession();
			var customers = GetAllCustomers(secondSession);

			Assert.That(customersWithOrderNumberEqualsTo2.Single(n => n.Id == Customer1.Id).Orders, Has.Count.EqualTo(Customer1.Orders.Count));

			using (IDbCommand cmd = OpenSession().Connection.CreateCommand())
			{
				cmd.CommandText = "DELETE FROM Orders;";
				cmd.ExecuteNonQuery();
				cmd.Connection.Close();
			}

			Assert.That(customers.Single(n => n.Id == Customer1.Id).Orders, Has.Count.EqualTo(Customer1.Orders.Count));
			Assert.That(customers.Single(n => n.Id == Customer2.Id).Orders, Has.Count.EqualTo(0));
			Assert.That(customers.Single(n => n.Id == Customer3.Id).Orders, Has.Count.EqualTo(0));

			firstSession.Dispose();
			secondSession.Dispose();
		}

		protected void ClearSecondLevelCacheFor(System.Type entity)
		{
			var entityName = entity.FullName;
			Sfi.EvictEntity(entityName);
			var entityPersister = Sfi.GetEntityPersister(entityName);
			if (!entityPersister.HasCache)
				return;

			IReadOnlyCollection<string> querySpaces = entityPersister.QuerySpaces;
			Sfi.UpdateTimestampsCache.Invalidate(querySpaces);
		}

		protected void ClearCollectionCache<T>(Expression<Func<T, IEnumerable>> pathToCollection)
		{
			var rootEntityTypeFullPath = typeof(T).FullName;
			var memberExpression = pathToCollection.Body as MemberExpression;
			if (memberExpression == null)
				throw new ArgumentException("pathToCollection should be member expression");

			var role = string.Format("{0}.{1}", rootEntityTypeFullPath, memberExpression.Member.Name);
			Sfi.EvictCollection(role);
		}

		protected abstract IList<Customer> GetCustomersWithFetchedOrdersWithoutRestrictions(ISession session);
		protected abstract IList<Customer> GetCustomersWithOrdersEagerLoaded(ISession session);
		protected abstract IList<Customer> GetCustomersByOrderNumberUsingOnClause(ISession session, int orderNumber);
		protected abstract IList<Customer> GetCustomersByOrderNumberUsingWhereClause(ISession session, int orderNumber);
		protected abstract IList<Customer> GetCustomersByNameUsingWhereClause(ISession session, string customerName);
		protected abstract IList<Customer> GetCustomersByOrderNumberUsingSubqueriesAndByNameUsingWhereClause(ISession session, int orderNumber, string customerName);
		protected abstract IList<Customer> GetAllCustomers(ISession session);
	}
}

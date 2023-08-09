using System;
using System.Collections.Generic;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3848
{
	public class HqlTestFixture : Fixture
	{
		[Ignore("We can't write such query using hql, because we can't use 'with' clause when we're fetching collection.")]
		public override void ChildCollectionsFromLeftOuterJoinWithOnClauseRestrictionOnCollectionShouldNotBeInSecondLevelCache()
		{
			throw new NotImplementedException();
		}

		[Ignore("We can't write such query using hql, because we can't use 'with' clause when we're fetching collection.")]
		public override void ChildCollectionsFromLeftOuterJoinOnlyWithRestrictionShouldNotBeIn2LvlCache()
		{
			throw new NotImplementedException();
		}

		[Ignore("We can't write such query using hql, because we can't use 'with' clause when we're fetching collection.")]
		public override void ChildCollectionsWithRestrictionShouldNotBeIn2LvlCache()
		{
			throw new NotImplementedException();
		}

		[Ignore("We can't write such query using hql, because we can't use 'with' clause when we're fetching collection.")]
		public override void ChildCollectionsWithSelectModeFetchOnCollectionShouldNotBeInSecondLevelCache()
		{
			throw new NotImplementedException();
		}

		protected override IList<Customer> GetCustomersWithOrdersEagerLoaded(ISession session)
		{
			var query = session.CreateQuery("from Customer as c left join fetch c.Orders as o");
			query.SetResultTransformer(new DistinctRootEntityResultTransformer());
			return query.List<Customer>();
		}

		//We can't write such query using hql, because we can't use 'with' clause when we're fetching collection.
		protected override IList<Customer> GetCustomersByOrderNumberUsingOnClause(ISession session, int orderNumber)
		{
			throw new NotImplementedException();
		}

		protected override IList<Customer> GetCustomersByOrderNumberUsingWhereClause(ISession session, int orderNumber)
		{
			var query = session.CreateQuery(
				"from Customer as c " +
				"left join fetch c.Orders as o " +
				"where o.Number = :number");
			query.SetParameter("number", orderNumber);
			return query.List<Customer>();
		}

		protected override IList<Customer> GetCustomersByNameUsingWhereClause(ISession session, string customerName)
		{
			var query = session.CreateQuery(
				"from Customer as c " +
				"left join fetch c.Orders as o " +
				"where c.Name = :name");
			query.SetParameter("name", customerName);
			query.SetResultTransformer(new DistinctRootEntityResultTransformer());
			return query.List<Customer>();
		}

		//We can't write such query using hql, because we can't use 'with' clause when we're fetching collection.
		protected override IList<Customer> GetCustomersByOrderNumberUsingFetch(ISession session, int orderNumber)
		{
			throw new NotImplementedException();
		}

		protected override IList<Customer> GetCustomersByOrderNumberUsingFetchAndWhereClause(ISession session, int orderNumber)
		{
			var query = session.CreateQuery(
				"from Customer as c " +
				"inner join fetch c.Orders as o " +
				"where o.Number = :number");
			query.SetParameter("number", orderNumber);
			return query.List<Customer>();
		}

		//We can't write such query using hql, because we can't use 'with' clause when we're fetching collection.
		protected override IList<Customer> GetCustomersAndCompaniesByOrderNumberUsingFetchAndWhereClause(ISession session, int orderNumber, string name)
		{
			throw new NotImplementedException();
		}

		protected override IList<Customer> GetCustomersAndCompaniesByOrderNumberUsingFetchWithoutRestrictions(ISession session)
		{
			var query = session.CreateQuery(
				"from Customer as c " +
				"inner join fetch c.Orders as o " +
				"left join fetch c.Companies as cmp");
			return query.List<Customer>();
		}

		protected override IList<Customer> GetCustomersByOrderNumberUsingSubqueriesAndByNameUsingWhereClause(ISession session, int orderNumber, string customerName)
		{
			var query = session.CreateQuery(
				"from Customer as c " +
				"left join fetch c.Orders as o " +
				"where c.Name = :name and " +
				"c.Id in (select c1.Id from Customer as c1 left join c1.Orders as o2 where o2.Number = :number)");
			query.SetParameter("name", customerName);
			query.SetParameter("number", orderNumber);
			query.SetResultTransformer(new DistinctRootEntityResultTransformer());
			return query.List<Customer>();
		}

		//We can't write such query using hql, because we can't use 'with' clause when we're fetching collection.
		protected override IList<Customer> GetCustomersWithCompaniesByOrderNumberUsingOnClause(ISession session, int orderNumber)
		{
			throw new NotImplementedException();
		}

		protected override IList<Customer> GetAllCustomers(ISession session)
		{
			return session.CreateQuery("from Customer as c").List<Customer>();
		}
	}
}

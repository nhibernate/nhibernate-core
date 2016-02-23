using System;
using System.Collections.Generic;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3848
{
	public class HqlTestFixture : TestFixture
	{
		[Ignore("We can't write such query using hql, because we can't use 'with' clause when we're fetching collection.")]
		public override void ChildCollectionsFromLeftOuterJoinWithOnClauseRestrictionOnCollectionShouldNotBeInSecondLevelCache()
		{
			throw new NotImplementedException();
		}

		protected override IList<Customer> GetCustomersWithFetchedOrdersWithoutRestrictions(ISession session)
		{
			var query = session.CreateQuery("from Customer as c left join fetch c.Orders as o");
			query.SetResultTransformer(new DistinctRootEntityResultTransformer());
			return query.List<Customer>();
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
			var numberParameterName = "number";
			var query = session.CreateQuery(string.Format("from Customer as c " +
														   "left join fetch c.Orders as o " +
														   "where o.Number = :{0}", numberParameterName));
			query.SetParameter(numberParameterName, orderNumber);
			return query.List<Customer>();
		}

		protected override IList<Customer> GetCustomersByNameUsingWhereClause(ISession session, string customerName)
		{
			var nameParameterName = "name";
			var query = session.CreateQuery(string.Format("from Customer as c " +
														  "left join fetch c.Orders as o " +
														  "where c.Name = :{0}", nameParameterName));
			query.SetParameter(nameParameterName, customerName);
			query.SetResultTransformer(new DistinctRootEntityResultTransformer());
			return query.List<Customer>();
		}

		protected override IList<Customer> GetCustomersByOrderNumberUsingSubqueriesAndByNameUsingWhereClause(ISession session, int orderNumber, string customerName)
		{
			var nameParameterName = "name";
			var numberParameterName = "number";
			var query = session.CreateQuery(string.Format("from Customer as c " +
														  "left join fetch c.Orders as o " +
														  "where c.Name = :{0} and " +
														  "c.Id in (select c1.Id from Customer as c1 left join c1.Orders as o2 where o2.Number = :{1})", nameParameterName, numberParameterName));
			query.SetParameter(nameParameterName, customerName);
			query.SetParameter(numberParameterName, orderNumber);
			query.SetResultTransformer(new DistinctRootEntityResultTransformer());
			return query.List<Customer>();
		}

		protected override IList<Customer> GetAllCustomers(ISession session)
		{
			return session.CreateQuery("from Customer as c").List<Customer>();
		}
	}
}

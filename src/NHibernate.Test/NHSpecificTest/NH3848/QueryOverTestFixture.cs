using System.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace NHibernate.Test.NHSpecificTest.NH3848
{
	public class QueryOverTestFixture : Fixture
	{
		protected override IList<Customer> GetCustomersWithFetchedOrdersWithoutRestrictions(ISession session)
		{
			Order ordersAlias = null;
			return session.QueryOver<Customer>()
								.JoinAlias(n => n.Orders,
											() => ordersAlias,
											JoinType.LeftOuterJoin)
								.TransformUsing(new DistinctRootEntityResultTransformer())
								.List();
		}

		protected override IList<Customer> GetCustomersWithOrdersEagerLoaded(ISession session)
		{
			return session.QueryOver<Customer>()
								.Fetch(SelectMode.Fetch, n => n.Orders)
								.TransformUsing(new DistinctRootEntityResultTransformer())
								.List();
		}

		protected override IList<Customer> GetCustomersByOrderNumberUsingOnClause(ISession session, int orderNumber)
		{
			Order ordersAlias = null;
			return session.QueryOver<Customer>()
								.JoinAlias(n => n.Orders,
											() => ordersAlias,
											JoinType.LeftOuterJoin,
											Restrictions.Eq("Number", orderNumber))
								.List();
		}

		protected override IList<Customer> GetCustomersWithCompaniesByOrderNumberUsingOnClause(ISession session, int orderNumber)
		{
			Order ordersAlias = null;
			Company companiesAlias = null;

			return session.QueryOver<Customer>()
								.JoinAlias(n => n.Orders,
											() => ordersAlias,
											JoinType.LeftOuterJoin,
											Restrictions.Eq("Number", orderNumber))
								.JoinAlias(n => n.Companies,
											() => companiesAlias,
											JoinType.LeftOuterJoin)
								.List();
		}

		protected override IList<Customer> GetCustomersByOrderNumberUsingWhereClause(ISession session, int orderNumber)
		{
			Order ordersAlias = null;
			return session.QueryOver<Customer>()
								.JoinQueryOver(n => n.Orders, () => ordersAlias, JoinType.LeftOuterJoin)
								.Where(Restrictions.Eq("Number", orderNumber))
								.List();
		}

		protected override IList<Customer> GetCustomersByNameUsingWhereClause(ISession session, string customerName)
		{
			Order ordersAlias = null;
			return session.QueryOver<Customer>()
						.JoinAlias(n => n.Orders,
									() => ordersAlias,
									JoinType.LeftOuterJoin
								)
						.Where(Restrictions.Eq("Name", customerName))
						.TransformUsing(new DistinctRootEntityResultTransformer())
						.List();
		}

		protected override IList<Customer> GetCustomersByOrderNumberUsingSubqueriesAndByNameUsingWhereClause(ISession session, int orderNumber, string customerName)
		{
			Order ordersAlias = null;
			Order ordersAlias2 = null;
			var subquery = QueryOver.Of<Customer>()
									.JoinAlias(n => n.Orders, () => ordersAlias, JoinType.LeftOuterJoin, Restrictions.Eq("Number", orderNumber))
									.Select(n => n.Id);

			return session.QueryOver<Customer>()
																.JoinAlias(n => n.Orders, () => ordersAlias2, JoinType.LeftOuterJoin)
																.WithSubquery.WhereProperty(n => n.Id).In(subquery)
																.Where(Restrictions.Eq("Name", customerName))
																.TransformUsing(new DistinctRootEntityResultTransformer())
																.List();
		}

		protected override IList<Customer> GetAllCustomers(ISession session)
		{
			return session.QueryOver<Customer>().List();
		}
	}
}

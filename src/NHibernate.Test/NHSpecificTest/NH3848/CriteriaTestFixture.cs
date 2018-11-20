using System.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace NHibernate.Test.NHSpecificTest.NH3848
{
	public class CriteriaTestFixture : Fixture
	{
		protected override IList<Customer> GetCustomersWithOrdersEagerLoaded(ISession session)
		{
			return
				session
					.CreateCriteria<Customer>()
					.Fetch("Orders")
					.SetResultTransformer(new DistinctRootEntityResultTransformer())
					.List<Customer>();
		}

		protected override IList<Customer> GetCustomersByOrderNumberUsingOnClause(ISession session, int orderNumber)
		{
			return
				session
					.CreateCriteria<Customer>()
					.CreateAlias("Orders", "order", JoinType.LeftOuterJoin, Restrictions.Eq("Number", orderNumber))
					.List<Customer>();
		}

		protected override IList<Customer> GetCustomersByOrderNumberUsingFetch(ISession session, int orderNumber)
		{
			return
				session
					.CreateCriteria<Customer>()
					.CreateAlias("Orders", "order", JoinType.InnerJoin, Restrictions.Eq("Number", orderNumber))
					.Fetch(SelectMode.Fetch, "Orders")
					.List<Customer>();
		}

		protected override IList<Customer> GetCustomersByOrderNumberUsingFetchAndWhereClause(ISession session, int orderNumber)
		{
			return
				session
					.CreateCriteria<Customer>()
					.Fetch(SelectMode.Fetch, "Orders")
					.CreateCriteria("Orders", JoinType.InnerJoin)
					.Add(Restrictions.Eq("Number", orderNumber))
					.List<Customer>();
		}

		protected override IList<Customer> GetCustomersAndCompaniesByOrderNumberUsingFetchAndWhereClause(ISession session, int orderNumber, string name)
		{
			return
				session
					.CreateCriteria<Customer>()
					.CreateAlias("Orders", "order", JoinType.InnerJoin, Restrictions.Eq("Number", orderNumber))
					.Fetch(SelectMode.Fetch, "Orders")
					.CreateAlias("Companies", "company", JoinType.LeftOuterJoin, Restrictions.Eq("Name", name))
					.List<Customer>();
		}

		protected override IList<Customer> GetCustomersAndCompaniesByOrderNumberUsingFetchWithoutRestrictions(ISession session)
		{
			return
				session
					.CreateCriteria<Customer>()
					.Fetch(SelectMode.Fetch, "Orders")
					.CreateAlias("Orders", "order", JoinType.InnerJoin)
					.CreateAlias("Companies", "company", JoinType.LeftOuterJoin)
					.List<Customer>();
		}

		protected override IList<Customer> GetCustomersWithCompaniesByOrderNumberUsingOnClause(
			ISession session,
			int orderNumber)
		{
			return
				session
					.CreateCriteria<Customer>()
					.CreateAlias("Orders", "order", JoinType.LeftOuterJoin, Restrictions.Eq("Number", orderNumber))
					.CreateAlias("Companies", "company", JoinType.LeftOuterJoin)
					.List<Customer>();
		}

		protected override IList<Customer> GetCustomersByOrderNumberUsingWhereClause(ISession session, int orderNumber)
		{
			return
				session
					.CreateCriteria<Customer>()
					.CreateCriteria("Orders", "Order", JoinType.LeftOuterJoin)
					.Add(Restrictions.Eq("Number", orderNumber))
					.SetResultTransformer(new DistinctRootEntityResultTransformer())
					.List<Customer>();
		}

		protected override IList<Customer> GetCustomersByNameUsingWhereClause(ISession session, string customerName)
		{
			return
				session
					.CreateCriteria<Customer>()
					.CreateAlias("Orders", "order", JoinType.LeftOuterJoin)
					.Add(Restrictions.Eq("Name", "First Customer"))
					.SetResultTransformer(new DistinctRootEntityResultTransformer())
					.List<Customer>();
		}

		protected override IList<Customer> GetCustomersByOrderNumberUsingSubqueriesAndByNameUsingWhereClause(
			ISession session,
			int orderNumber,
			string customerName)
		{
			var detachedCriteria =
				DetachedCriteria
					.For<Customer>()
					.CreateAlias("Orders", "order", JoinType.LeftOuterJoin, Restrictions.Eq("Number", orderNumber))
					.SetProjection(Projections.Id());

			return
				session
					.CreateCriteria<Customer>()
					.CreateAlias("Orders", "order1", JoinType.LeftOuterJoin)
					.Add(Subqueries.PropertyIn("Id", detachedCriteria))
					.Add(Restrictions.Eq("Name", customerName))
					.SetResultTransformer(new DistinctRootEntityResultTransformer())
					.List<Customer>();
		}

		protected override IList<Customer> GetAllCustomers(ISession session)
		{
			return session.CreateCriteria<Customer>().List<Customer>();
		}
	}
}

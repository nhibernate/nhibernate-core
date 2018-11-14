using System.Collections.Generic;
using System.Threading.Tasks;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace NHibernate.Test.NHSpecificTest.NH3848
{
    public class CriteriaTestFixture : TestFixture
    {
        protected override IList<Customer> GetCustomersWithFetchedOrdersWithoutRestrictions(ISession session)
        {
            return session.CreateCriteria<Customer>()
                          .CreateAlias("Orders", "order", JoinType.LeftOuterJoin)
                          .SetResultTransformer(new DistinctRootEntityResultTransformer())
                          .List<Customer>();
        }

        protected override IList<Customer> GetCustomersWithOrdersEagerLoaded(ISession session)
        {
            return session.CreateCriteria<Customer>()
					      .Fetch("Orders")
						  .SetResultTransformer(new DistinctRootEntityResultTransformer())
                          .List<Customer>();
        }

		protected override async Task<IList<Customer>> GetCustomersWithOrdersEagerLoadedAsync(ISession session)
		{
			return await session.CreateCriteria<Customer>()
						  .Fetch("Orders")
						  .SetResultTransformer(new DistinctRootEntityResultTransformer())
						  .ListAsync<Customer>().ConfigureAwait(false);
		}

		protected override IList<Customer> GetCustomersByOrderNumberUsingOnClause(ISession session, int orderNumber)
        {
            return session.CreateCriteria<Customer>()
                            .CreateAlias("Orders", "order", JoinType.LeftOuterJoin, Restrictions.Eq("Number", orderNumber))
                            .List<Customer>();
        }

		protected override async Task<IList<Customer>> GetCustomersByOrderNumberUsingOnClauseAsync(ISession session, int orderNumber)
		{
			return await session.CreateCriteria<Customer>()
							.CreateAlias("Orders", "order", JoinType.LeftOuterJoin, Restrictions.Eq("Number", orderNumber))
							.ListAsync<Customer>().ConfigureAwait(false);
		}

		protected override IList<Customer> GetCustomersWithCompaniesByOrderNumberUsingOnClause(ISession session, int orderNumber)
		{
			return session.CreateCriteria<Customer>()
							.CreateAlias("Orders", "order", JoinType.LeftOuterJoin, Restrictions.Eq("Number", orderNumber))
							.CreateAlias("Companies", "company", JoinType.LeftOuterJoin)
							.List<Customer>();
		}

		protected override async Task<IList<Customer>> GetCustomersWithCompaniesByOrderNumberUsingOnClauseAsync(ISession session, int orderNumber)
		{
			return await session.CreateCriteria<Customer>()
							.CreateAlias("Orders", "order", JoinType.LeftOuterJoin, Restrictions.Eq("Number", orderNumber))
							.CreateAlias("Companies", "company", JoinType.LeftOuterJoin)
							.ListAsync<Customer>().ConfigureAwait(false);
		}

		protected override IList<Customer> GetCustomersByOrderNumberUsingWhereClause(ISession session, int orderNumber)
        {
            return session.CreateCriteria<Customer>()
                            .CreateCriteria("Orders", "Order", JoinType.LeftOuterJoin)
                                    .Add(Restrictions.Eq("Number", orderNumber))
                                .SetResultTransformer(new DistinctRootEntityResultTransformer())
                            .List<Customer>();
        }

		protected override async Task<IList<Customer>> GetCustomersByOrderNumberUsingWhereClauseAsync(ISession session, int orderNumber)
		{
			return await session.CreateCriteria<Customer>()
							.CreateCriteria("Orders", "Order", JoinType.LeftOuterJoin)
							.Add(Restrictions.Eq("Number", orderNumber))
							.SetResultTransformer(new DistinctRootEntityResultTransformer())
							.ListAsync<Customer>().ConfigureAwait(false);
		}

		protected override IList<Customer> GetCustomersByNameUsingWhereClause(ISession session, string customerName)
        {
            return session.CreateCriteria<Customer>()
                            .CreateAlias("Orders", "order", JoinType.LeftOuterJoin)
                                .Add(Restrictions.Eq("Name", "First Customer"))
                            .SetResultTransformer(new DistinctRootEntityResultTransformer())
                            .List<Customer>();
        }

		protected override async Task<IList<Customer>> GetCustomersByNameUsingWhereClauseAsync(ISession session, string customerName)
		{
			return await session.CreateCriteria<Customer>()
							.CreateAlias("Orders", "order", JoinType.LeftOuterJoin)
							.Add(Restrictions.Eq("Name", "First Customer"))
							.SetResultTransformer(new DistinctRootEntityResultTransformer())
							.ListAsync<Customer>().ConfigureAwait(false);
		}

		protected override IList<Customer> GetCustomersByOrderNumberUsingSubqueriesAndByNameUsingWhereClause(ISession session, int orderNumber, string customerName)
        {
            var detachedCriteria = DetachedCriteria.For<Customer>()
                                                         .CreateAlias("Orders", "order", JoinType.LeftOuterJoin, Restrictions.Eq("Number", orderNumber))
                                                         .SetProjection(Projections.Id());

            return session.CreateCriteria<Customer>()
                                .CreateAlias("Orders", "order1", JoinType.LeftOuterJoin)
                                .Add(Subqueries.PropertyIn("Id",detachedCriteria))
                                .Add(Restrictions.Eq("Name", customerName))
                                .SetResultTransformer(new DistinctRootEntityResultTransformer())
                                .List<Customer>();
        }


		protected override async Task<IList<Customer>> GetCustomersByOrderNumberUsingSubqueriesAndByNameUsingWhereClauseAsync(ISession session, int orderNumber, string customerName)
		{
			var detachedCriteria = DetachedCriteria.For<Customer>()
													  .CreateAlias("Orders", "order", JoinType.LeftOuterJoin, Restrictions.Eq("Number", orderNumber))
													  .SetProjection(Projections.Id());

			return await session.CreateCriteria<Customer>()
								.CreateAlias("Orders", "order1", JoinType.LeftOuterJoin)
								.Add(Subqueries.PropertyIn("Id", detachedCriteria))
								.Add(Restrictions.Eq("Name", customerName))
								.SetResultTransformer(new DistinctRootEntityResultTransformer())
								.ListAsync<Customer>().ConfigureAwait(false);
		}

		protected override IList<Customer> GetAllCustomers(ISession session)
        {
            return session.CreateCriteria<Customer>().List<Customer>();
        }

		protected override async Task<IList<Customer>> GetAllCustomersAsync(ISession session)
		{
			return await session.CreateCriteria<Customer>().ListAsync<Customer>().ConfigureAwait(false);
		}
	}
}

using System.Linq;
using NHibernate.Linq;

namespace NHibernate.Test.Linq.Entities
{
    public class Northwind
    {
        private readonly ISession _session;

        public Northwind(ISession session)
        {
            _session = session;
        }

        public IQueryable<Customer> Customers
        {
            get { return _session.Query<Customer>(); }
        }

        public IQueryable<Product> Products
        {
            get { return _session.Query<Product>(); }
        }

        public IQueryable<Order> Orders
        {
            get { return _session.Query<Order>(); }
        }
		
        public IQueryable<OrderLine> OrderLines
        {
            get { return _session.Query<OrderLine>(); }
        }

        public IQueryable<Employee> Employees
        {
            get { return _session.Query<Employee>(); }
        }
    }
}
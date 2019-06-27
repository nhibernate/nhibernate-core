using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NHibernate.DomainModel.Northwind.Entities
{
    public class Customer
    {
        

        public virtual string CustomerId { get; set; }

        public virtual string CompanyName { get; set; }

        public virtual string ContactName { get; set; }

        public virtual string ContactTitle { get; set; }

        public virtual Address Address { get; set; }

        public virtual ISet<Order> Orders => new HashSet<Order>();

        public virtual void AddOrder(Order order)
        {
            if (!Orders.Contains(order))
            {
	            Orders.Add(order);
                order.Customer = this;
            }
        }

        public virtual void RemoveOrder(Order order)
        {
            if (Orders.Contains(order))
            {
	            Orders.Remove(order);
                order.Customer = null;
            }
        }
    }
}
using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3848
{
    public class Customer
    {
        public virtual Guid Id { get; set; }
        public virtual ISet<Order> Orders { get; set; }
        public virtual string Name { get; set; }

        public Customer()
        {
            Orders = new HashSet<Order>();
        }

        public virtual void AddOrder(Order order)
        {
            Orders.Add(order);
            order.Customer = this;
        }
    }
}

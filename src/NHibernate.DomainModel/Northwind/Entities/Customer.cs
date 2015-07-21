using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NHibernate.DomainModel.Northwind.Entities
{
    public class Customer
    {
        private readonly ISet<Order> _orders;
        private Address _address;
        private string _companyName;
        private string _contactName;
        private string _contactTitle;
        private string _customerId;

        public Customer()
        {
            _orders = new HashSet<Order>();
        }

        public virtual string CustomerId
        {
            get { return _customerId; }
            set { _customerId = value; }
        }

        public virtual string CompanyName
        {
            get { return _companyName; }
            set { _companyName = value; }
        }

        public virtual string ContactName
        {
            get { return _contactName; }
            set { _contactName = value; }
        }

        public virtual string ContactTitle
        {
            get { return _contactTitle; }
            set { _contactTitle = value; }
        }

        public virtual Address Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public virtual ISet<Order> Orders
        {
            get { return _orders; }
        }

        public virtual void AddOrder(Order order)
        {
            if (!_orders.Contains(order))
            {
                _orders.Add(order);
                order.Customer = this;
            }
        }

        public virtual void RemoveOrder(Order order)
        {
            if (_orders.Contains(order))
            {
                _orders.Remove(order);
                order.Customer = null;
            }
        }
    }
}
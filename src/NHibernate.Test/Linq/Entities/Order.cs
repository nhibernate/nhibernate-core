using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Iesi.Collections.Generic;

namespace NHibernate.Test.Linq.Entities
{
    public class Order : Entity<Order>
    {
        private readonly ISet<OrderLine> _orderLines;
        private Customer _customer;
        private Employee _employee;
        private decimal? _freight;
        private DateTime? _orderDate;
        private DateTime? _requiredDate;
        private string _shippedTo;
        private Shipper _shipper;
        private Address _shippingAddress;
        private DateTime? _shippingDate;

        public Order() : this(null)
        {
        }

        public Order(Customer customer)
        {
            _orderLines = new HashedSet<OrderLine>();

            _customer = customer;
        }

        public virtual Customer Customer
        {
            get { return _customer; }
            set { _customer = value; }
        }

        public virtual Employee Employee
        {
            get { return _employee; }
            set { _employee = value; }
        }

        public virtual DateTime? OrderDate
        {
            get { return _orderDate; }
            set { _orderDate = value; }
        }

        public virtual DateTime? RequiredDate
        {
            get { return _requiredDate; }
            set { _requiredDate = value; }
        }

        public virtual DateTime? ShippingDate
        {
            get { return _shippingDate; }
            set { _shippingDate = value; }
        }

        public virtual Shipper Shipper
        {
            get { return _shipper; }
            set { _shipper = value; }
        }

        public virtual decimal? Freight
        {
            get { return _freight; }
            set { _freight = value; }
        }

        public virtual string ShippedTo
        {
            get { return _shippedTo; }
            set { _shippedTo = value; }
        }

        public virtual Address ShippingAddress
        {
            get { return _shippingAddress; }
            set { _shippingAddress = value; }
        }

        public virtual ReadOnlyCollection<OrderLine> OrderLines
        {
            get { return new ReadOnlyCollection<OrderLine>(new List<OrderLine>(_orderLines)); }
        }

        public virtual void AddOrderLine(OrderLine orderLine)
        {
            if (!_orderLines.Contains(orderLine))
            {
                orderLine.Order = this;
                _orderLines.Add(orderLine);
            }
        }

        public virtual void RemoveOrderLine(OrderLine orderLine)
        {
            if (_orderLines.Contains(orderLine))
            {
                _orderLines.Remove(orderLine);
                orderLine.Order = null;
            }
        }
    }
}
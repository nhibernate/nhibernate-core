using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NHibernate.DomainModel.Northwind.Entities
{
    public class Order
    {
        private readonly ISet<OrderLine> _orderLines;

        public Order()         
        {
            _orderLines = new HashSet<OrderLine>();
        }

        public virtual int OrderId { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual DateTime? OrderDate { get; set; }

        public virtual DateTime? RequiredDate { get; set; }

        public virtual DateTime? ShippingDate { get; set; }

        public virtual Shipper Shipper { get; set; }

        public virtual decimal? Freight { get; set; }

        public virtual string ShippedTo { get; set; }

        public virtual Address ShippingAddress { get; set; }

        public virtual ISet<OrderLine> OrderLines => _orderLines;

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
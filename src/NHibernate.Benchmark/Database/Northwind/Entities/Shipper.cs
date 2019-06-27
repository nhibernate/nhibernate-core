using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NHibernate.DomainModel.Northwind.Entities
{
    public class Shipper
    {
        private readonly IList<Order> _orders;

        public Shipper() 
        {
            _orders = new List<Order>();
        }

        public virtual int ShipperId { get; set; }

        public virtual string CompanyName { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual Guid Reference { get; set; }

        public virtual ReadOnlyCollection<Order> Orders => new ReadOnlyCollection<Order>(_orders);
    }
}
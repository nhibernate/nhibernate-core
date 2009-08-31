using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NHibernate.Test.Linq.Entities
{
    public class Shipper
    {
        private readonly IList<Order> _orders;
        private string _companyName;
        private string _phoneNumber;
        private int _shipperId;

        public Shipper() 
        {
            _orders = new List<Order>();
        }

        public virtual int ShipperId
        {
            get { return _shipperId; }
            set { _shipperId = value; }
        }

        public virtual string CompanyName
        {
            get { return _companyName; }
            set { _companyName = value; }
        }

        public virtual string PhoneNumber
        {
            get { return _phoneNumber; }
            set { _phoneNumber = value; }
        }

        public virtual ReadOnlyCollection<Order> Orders
        {
            get { return new ReadOnlyCollection<Order>(_orders); }
        }
    }
}
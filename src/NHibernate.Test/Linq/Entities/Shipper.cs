using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NHibernate.Test.Linq.Entities
{
    public class Shipper : Entity<Shipper>
    {
        private readonly IList<Order> _orders;
        private string _companyName;
        private string _phoneNumber;

        public Shipper() : this(null)
        {
        }

        public Shipper(string companyName)
        {
            _companyName = companyName;

            _orders = new List<Order>();
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
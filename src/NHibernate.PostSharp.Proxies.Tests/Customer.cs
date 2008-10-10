using System.Collections.Generic;

namespace NHibernate.PostSharp.Proxies.Tests
{
    [NHibernateLazyLoadingSupport, AddNHibernateProxyAttribute]
    public class Customer
    {
        public Customer()
        {
            Addresses = new List<Address>();
        }

        public string Name { get; set; }
        public int Id { get; set; }
        public ICollection<Address> Addresses { get; set; }

        public void AddAddress(Address address)
        {
            address.Customer = this;
            Addresses.Add(address);
        }
    }
}

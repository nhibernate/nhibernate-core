using System.Collections.Generic;

namespace NHibernate.PostSharp.Proxies.Tests
{
    // those attributes are here until I figure out how to make PostSharp get the list of entities
    // we want to weave externally, hopefully from the mapping

    // THEY WON'T BE HERE FOR FINAL VERSION - THIS CLASS WOULD BE FULLY POCO
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

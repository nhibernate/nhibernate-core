namespace NHibernate.PostSharp.Proxies.Tests
{
    // those attributes are here until I figure out how to make PostSharp get the list of entities
    // we want to weave externally, hopefully from the mapping

    // THEY WON'T BE HERE FOR FINAL VERSION - THIS CLASS WOULD BE FULLY POCO
    [NHibernateLazyLoadingSupport, AddNHibernateProxyAttribute]
    public class Address
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Home { get; set; }
    }
}
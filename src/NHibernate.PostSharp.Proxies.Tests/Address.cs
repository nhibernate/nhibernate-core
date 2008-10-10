namespace NHibernate.PostSharp.Proxies.Tests
{
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
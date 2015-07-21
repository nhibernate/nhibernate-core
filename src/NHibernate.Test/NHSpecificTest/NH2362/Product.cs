namespace NHibernate.Test.NHSpecificTest.NH2362
{
    public class Product
    {
        public virtual int Id { get; set; }
        public virtual Category Category { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual string Name { get; set; }
        public virtual decimal Price { get; set; }
    }
}

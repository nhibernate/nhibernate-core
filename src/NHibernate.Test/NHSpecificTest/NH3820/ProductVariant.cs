namespace NHibernate.Test.NHSpecificTest.NH3820
{
    public class ProductVariant
    {
        public ProductVariant(string name, string code, Product product)
            : this()
        {
            this.Name = name;
            this.Code = code;
            this.Product = product;
        }

        public ProductVariant()
        {
        }

        public virtual string Code { get; protected set; }

        public virtual int Id { get; protected set; }

        public virtual string Name { get; protected set; }

        public virtual Product Product { get; protected set; }
    }
}

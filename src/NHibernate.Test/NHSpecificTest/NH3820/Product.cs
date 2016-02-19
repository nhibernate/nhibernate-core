namespace NHibernate.Test.NHSpecificTest.NH3820
{
    #region using

    using System.Collections.Generic;

    #endregion

    public class Product
    {
        public Product(string code, string name)
            : this()
        {
            this.Code = code;
            this.Name = name;
        }

        public Product()
        {
            this.Variants = new List<ProductVariant>();
        }

        public virtual string Code { get; protected set; }

        public virtual int Id { get; protected set; }

        public virtual string Name { get; protected set; }

        public virtual ICollection<ProductVariant> Variants { get; protected set; }

        public virtual void AddVariant(ProductVariant variant)
        {
            this.Variants.Add(variant);
        }
    }
}

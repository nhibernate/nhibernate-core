using NHibernate;
using NHibernate.Mapping;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.Test.NHSpecificTest.GH2688
{
    public class Product
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual decimal Price { get; set; }

        public virtual IList<BasketItem> BasketItems { get; set; }
    }

    public class ProductMap : ClassMapping<Product>
    {
        public ProductMap()
        {
            Id(x => x.Id, m => {
                m.Generator(Generators.Identity);
                m.Column("ProductId");
            });
            Property(x => x.Name);
            Property(x => x.Price);
            Bag(x => x.BasketItems, bag => {
                bag.Inverse(true);
                bag.Lazy(CollectionLazy.Lazy);
                bag.Cascade(Mapping.ByCode.Cascade.Persist);
				bag.OrderBy("OrderBy asc");
                bag.Key(km =>
                {
                    km.Column("ProductId");
                    km.ForeignKey("none");
                });
            }, a => a.OneToMany());
        }
    }
}

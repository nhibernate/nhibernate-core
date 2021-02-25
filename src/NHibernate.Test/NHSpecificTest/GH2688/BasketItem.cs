using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.Test.NHSpecificTest.GH2688
{
    public class BasketItem
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual decimal Price { get; set; }

        public virtual Product Product { get; set; }

		public virtual int OrderBy { get; set; }
	}

    public class BasketItemMap : ClassMapping<BasketItem>
    {
        public BasketItemMap()
        {
            Id(x => x.Id, m => {
                m.Generator(Generators.Identity);
                m.Column("BasketItemId");
            });
            Property(x => x.Name);
            Property(x => x.Price);
			Property(x => x.OrderBy);
            ManyToOne(x => x.Product, m => {
                m.NotFound(NotFoundMode.Ignore);
                m.Column("ProductId");
            });
        }
    }
}

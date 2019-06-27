using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NHibernate.DomainModel.Northwind.Entities
{
    public class ProductCategory
    {
        private readonly ISet<Product> _products;

        public ProductCategory() 
        {
            _products = new HashSet<Product>();
        }

        public virtual int CategoryId { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual ReadOnlyCollection<Product> Products => new ReadOnlyCollection<Product>(new List<Product>(_products));

        public virtual void AddProduct(Product product)
        {
            if (!_products.Contains(product))
            {
                _products.Add(product);
                product.Category = this;
            }
        }

        public virtual void RemoveProduct(Product product)
        {
            if (_products.Contains(product))
            {
                _products.Remove(product);
                product.Category = null;
            }
        }
    }
}
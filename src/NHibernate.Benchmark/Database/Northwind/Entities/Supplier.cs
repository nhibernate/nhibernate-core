using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NHibernate.DomainModel.Northwind.Entities
{
    public class Supplier
    {
        private readonly ISet<Product> _products;

        public Supplier() 
        {
            _products = new HashSet<Product>();
        }

        public virtual int SupplierId { get; set; }

        public virtual string CompanyName { get; set; }

        public virtual string ContactName { get; set; }

        public virtual string ContactTitle { get; set; }

        public virtual string HomePage { get; set; }

        public virtual Address Address { get; set; }

        public virtual ReadOnlyCollection<Product> Products => new ReadOnlyCollection<Product>(new List<Product>(_products));

        public virtual void AddProduct(Product product)
        {
            if (!_products.Contains(product))
            {
                _products.Add(product);
                product.Supplier = this;
            }
        }

        public virtual void RemoveProduct(Product product)
        {
            if (_products.Contains(product))
            {
                _products.Remove(product);
                product.Supplier = null;
            }
        }
    }
}
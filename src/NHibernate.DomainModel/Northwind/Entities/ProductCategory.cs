using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NHibernate.DomainModel.Northwind.Entities
{
    public class ProductCategory
    {
        private readonly ISet<Product> _products;
        private string _description;
        private string _name;
        private int _categoryId;

        public ProductCategory() 
        {
            _products = new HashSet<Product>();
        }

        public virtual int CategoryId
        {
            get { return _categoryId; }
            set { _categoryId = value; }
        }

        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public virtual string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public virtual ReadOnlyCollection<Product> Products
        {
            get { return new ReadOnlyCollection<Product>(new List<Product>(_products)); }
        }

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
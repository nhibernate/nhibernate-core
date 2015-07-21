using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NHibernate.DomainModel.Northwind.Entities
{
    public class Supplier
    {
        private readonly ISet<Product> _products;
        private Address _address;
        private string _companyName;
        private string _contactName;
        private string _contactTitle;
        private string _homePage;
        private int _supplierId;

        public Supplier() 
        {
            _products = new HashSet<Product>();
        }

        public virtual int SupplierId 
        {
            get { return _supplierId; }
            set { _supplierId = value; }
        }

        public virtual string CompanyName
        {
            get { return _companyName; }
            set { _companyName = value; }
        }

        public virtual string ContactName
        {
            get { return _contactName; }
            set { _contactName = value; }
        }

        public virtual string ContactTitle
        {
            get { return _contactTitle; }
            set { _contactTitle = value; }
        }

        public virtual string HomePage
        {
            get { return _homePage; }
            set { _homePage = value; }
        }

        public virtual Address Address
        {
            get { return _address; }
            set { _address = value; }
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
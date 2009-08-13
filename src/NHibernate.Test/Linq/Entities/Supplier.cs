using System.Collections.Generic;
using System.Collections.ObjectModel;
using Iesi.Collections.Generic;

namespace NHibernate.Test.Linq.Entities
{
    public class Supplier : Entity<Supplier>
    {
        private readonly ISet<Product> _products;
        private Address _address;
        private string _companyName;
        private string _contactName;
        private string _contactTitle;
        private string _homePage;

        public Supplier() : this(null)
        {
        }

        public Supplier(string companyName)
        {
            _products = new HashedSet<Product>();

            _companyName = companyName;
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
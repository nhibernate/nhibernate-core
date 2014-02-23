using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3571
{
	public class Product
	{
		public Product()
		{
			Details = new ProductDetails();
		}
		public virtual string ProductId { get; set; }

		public virtual ProductDetails Details
		{
			get;
			set;
		}

		public virtual IList<ProductDetails> DetailsList
		{
			get;
			set;
		}
	}

	public class ProductDetails
	{
		private IDictionary _properties;

		public virtual IDictionary Properties
		{
			get
			{
				if (_properties == null)
					_properties = new Hashtable();

				return _properties;
			}
			set { _properties = value; }
		}
	}
}
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3571Generic
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
		private IDictionary<string, object> _properties;

		public virtual IDictionary<string, object> Properties
		{
			get
			{
				if (_properties == null)
					_properties = new Dictionary<string, object>();

				return _properties;
			}
			set { _properties = value; }
		}
	}
}

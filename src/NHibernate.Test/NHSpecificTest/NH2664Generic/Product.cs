using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2664Generic
{
	public class Product
	{
		public virtual string ProductId { get; set; }

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

using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH2664
{
	public class Product
	{
		public virtual string ProductId { get; set; }

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
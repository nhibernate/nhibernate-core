using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1579
{
	public class Cart : Entity
	{
		public Cart(string vendorName)
		{
			if (String.IsNullOrEmpty(vendorName))
				throw new ArgumentNullException("vendorName");

			VendorName = vendorName;
			EnsureCollections();
		}

		private void EnsureCollections()
		{
			if(Apples == null)
				Apples = new List<Apple>();
			if(Oranges == null)
				Oranges = new List<Orange>();
		}

		protected Cart()
		{
		}

		public string VendorName { get; set; }
		public IList<Apple> Apples { get; set; }
		public IList<Orange> Oranges { get; set; }
	}
}

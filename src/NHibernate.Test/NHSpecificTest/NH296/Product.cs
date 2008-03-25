using System;

namespace NHibernate.Test.NHSpecificTest.NH296
{
	public class Product
	{
		private ProductPK _productPK;

		public ProductPK ProductPK
		{
			get { return _productPK; }
			set { _productPK = value; }
		}
	}
}
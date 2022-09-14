using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3571Generic
{
	public interface IProduct
	{
		ProductDetails Details { get; set; }
		IList<ProductDetails> DetailsList { get; set; }
		string ProductId { get; set; }
	}
}

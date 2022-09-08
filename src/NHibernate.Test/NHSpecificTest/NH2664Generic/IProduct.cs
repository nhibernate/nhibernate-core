using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2664Generic
{
	public interface IProduct
	{
		string ProductId { get; set; }
		IDictionary<string, object> Properties { get; set; }
	}
}
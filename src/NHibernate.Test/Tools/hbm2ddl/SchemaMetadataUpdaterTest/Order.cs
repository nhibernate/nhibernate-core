using System.Collections.Generic;

namespace NHibernate.Test.Tools.hbm2ddl.SchemaMetadataUpdaterTest
{
	public class Order
	{
		public string Select { get; set; }
		public string From { get; set; }
		public string And { get; set; }
		public string Column { get; set; }
		public string Name { get; set; }
		public string Abracadabra { get; set; }
		public ISet<OrderRow> Rows { get; set; }
	}
}

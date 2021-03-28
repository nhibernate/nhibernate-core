using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.Test.NHSpecificTest.GH2029
{
	public class TestClass
	{
		public virtual int Id { get; set; }
		public virtual int? NullableInt32Prop { get; set; }
		public virtual int Int32Prop { get; set; }
		public virtual long? NullableInt64Prop { get; set; }
		public virtual long Int64Prop { get; set; }
	}
}

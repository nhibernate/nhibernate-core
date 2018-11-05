using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH1824
{
	public class Bar
	{
		public virtual int Id { get; set; }

		public virtual IDictionary MyProps { get; set; } = new Dictionary<string, object>();

		public virtual StaticBarComponent StaticProps { get; set; }
	}

	public class StaticBarComponent
	{
		public string StaticValString2 { get; set; }
		public string YetAnotherProperty { get; set; }
	}
}

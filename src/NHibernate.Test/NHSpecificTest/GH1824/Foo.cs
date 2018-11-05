using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH1824
{
	public class Foo
	{
		public virtual int Id { get; set; }

		public virtual IDictionary MyProps { get; set; } = new Dictionary<string, object>();

		public virtual StaticFooComponent StaticProps { get; set; }
	}

	public class StaticFooComponent
	{
		public Bar StaticPointer { get; set; }
	}
}

using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH1766
{
	public class A
	{
		public virtual Guid Id { get; set; }
		public virtual IList<B> Items { get; set; }
	}

	public class B
	{
		public virtual Guid Id { get; set; }
		public virtual A A { get; set; }
		public virtual int ListIndex { get; set; }
	}
}

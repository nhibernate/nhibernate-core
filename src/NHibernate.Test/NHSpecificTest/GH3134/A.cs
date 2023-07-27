using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH3134
{
	public class A
	{
		public virtual Guid Id { get; set; }
		public virtual string NameA { get; set; }
		public virtual ISet<B> Bs { get; set; } = new HashSet<B>();
	}
}

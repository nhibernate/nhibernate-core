using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH3134
{
	public class B
	{
		public virtual Guid Id { get; set; }
		public virtual string NameB { get; set; }
		public virtual ISet<A> As { get; set; } = new HashSet<A>();
	}
}

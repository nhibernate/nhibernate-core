using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1182
{
	public class ObjectA
	{
		public virtual int Id { get; set; }
		public virtual DateTime Version { get; set; }
		public virtual IList<ObjectB> Bs { get; set; }
	}

	public class ObjectB
	{
		public virtual int Id { get; set; }
		public virtual DateTime Version { get; set; }
	}
}
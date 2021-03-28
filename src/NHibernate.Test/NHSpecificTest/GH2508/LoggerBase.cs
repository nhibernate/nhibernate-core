using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH2508
{
	public abstract class LoggerBase
	{
		public LoggerBase()
		{
			Children = new List<Child>();
		}

		public virtual string Solution { get; set; }
	
		public virtual Guid Id { get; set; }

		public virtual string Name { get; set; }

		public virtual IList<Child> Children { get; set; }
	}
}

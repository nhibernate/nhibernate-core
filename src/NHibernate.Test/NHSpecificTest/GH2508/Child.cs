using System;

namespace NHibernate.Test.NHSpecificTest.GH2508
{
	public class Child
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual LoggerBase Logger { get; set; }
	}
}

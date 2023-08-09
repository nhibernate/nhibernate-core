using System;

namespace NHibernate.Test.NHSpecificTest.GH2627
{
	public class Child
	{
		public virtual Guid Id { get; set; }
		public virtual Entity Parent { get; set; }
		public virtual string Name { get; set; }
	}
}

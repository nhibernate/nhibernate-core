using System;

namespace NHibernate.Test.NHSpecificTest.NH2331
{
	public class Forum
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual double Dollars { get; set; }
	}
}

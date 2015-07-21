using System;

namespace NHibernate.Test.NHSpecificTest.NH2251
{
	public class Foo
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual int Ord { get; set; }
	}
}
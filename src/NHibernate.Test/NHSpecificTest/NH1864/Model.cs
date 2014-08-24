using System;

namespace NHibernate.Test.NHSpecificTest.NH1864
{
	public class Category
	{
		public virtual int ID { get; protected set; }
		public virtual DateTime ValidUntil { get; set; }
	}

	public class Invoice
	{
		public virtual int ID { get; protected set; }
		public virtual DateTime ValidUntil { get; set; }
		public virtual int Foo { get; set; }
	}
}

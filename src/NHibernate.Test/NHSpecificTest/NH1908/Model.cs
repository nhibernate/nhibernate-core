using System;

namespace NHibernate.Test.NHSpecificTest.NH1908
{
	public class Category
	{
		public virtual int ID { get; protected set; }
		public virtual Category ParentCategory { get; set; }
		public virtual DateTime ValidUntil { get; set; }
	}

	public class Invoice
	{
		public virtual int ID { get; protected set; }
		public virtual DateTime Issued { get; set; }
		public virtual Category Category { get; set; }
	}
}

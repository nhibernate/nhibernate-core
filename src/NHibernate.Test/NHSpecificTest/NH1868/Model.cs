using System;

namespace NHibernate.Test.NHSpecificTest.NH1868
{
	public class Category
	{
		public virtual int ID { get; protected set; }
		public virtual DateTime ValidUntil { get; set; }
	}

	public class Package
	{
		public virtual int ID { get; protected set; }
		public virtual DateTime ValidUntil { get; set; }
	}

	public class Invoice
	{
		public virtual int ID { get; protected set; }
		public virtual DateTime ValidUntil { get; set; }
		public virtual Category Category { get; set; }
		public virtual Package Package { get; set; }
	}
}

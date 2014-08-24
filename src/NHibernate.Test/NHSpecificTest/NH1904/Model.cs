using System;

namespace NHibernate.Test.NHSpecificTest.NH1904
{
	public class Invoice
	{
		public virtual int ID { get; protected set; }
		public virtual DateTime Issued { get; set; }

		protected virtual DateTime issued { get; set; }
	}
}

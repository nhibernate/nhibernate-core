using System;

namespace NHibernate.Test.NHSpecificTest.GH2201
{
	public class Order
	{
		public virtual int Id { get; set; }
		public virtual Person Person { get; set; }
		public virtual DateTime Date { get; set; }
	}
}

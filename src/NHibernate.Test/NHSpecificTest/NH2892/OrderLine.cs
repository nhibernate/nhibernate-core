using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2892
{
	public class OrderLine
	{
		public virtual int Id { get; set; }
		public virtual Order Orders { get; set; }
	}
}

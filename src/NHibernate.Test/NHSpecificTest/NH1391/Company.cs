using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1391
{
	public class Company
	{
		public virtual int Id { get; set; }
		public virtual IList<ProductA> Products { get; set; }
	}
}

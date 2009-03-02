using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1391
{
	public class Product
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Company Company { get; set; }
	}
	public class ProductA:Product
	{
		
	}
	public class ProductB:Product
	{
		
	}
}

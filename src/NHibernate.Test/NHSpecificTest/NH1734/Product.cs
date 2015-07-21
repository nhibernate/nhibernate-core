using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1734
{
	public class Product
	{
		public virtual int Id { get; set; }
		public virtual double Price { get; set; }
		public virtual int Amount { get; set; }
	}
}

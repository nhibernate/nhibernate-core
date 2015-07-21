using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1391
{
	public class Animal
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Person Owner { get; set; }
	}
}

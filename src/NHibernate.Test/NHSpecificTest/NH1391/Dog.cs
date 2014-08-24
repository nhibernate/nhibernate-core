using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1391
{
	public class Dog:Animal
	{
		public virtual string Country { get; set; }
	}
}

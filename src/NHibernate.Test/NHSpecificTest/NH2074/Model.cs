using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2074
{
	public class Person
	{ 
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual int CalculatedProperty { get; set; }
	} 

}

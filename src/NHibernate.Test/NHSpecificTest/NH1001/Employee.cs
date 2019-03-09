using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1001
{
	public class Employee
	{
		public virtual int Id { get; set; }
		public virtual string FirstName { get; set; }
		public virtual string LastName { get; set; }
		public virtual Department Department1 { get; set; }
		public virtual Department Department2 { get; set; }
		public virtual Department Department3 { get; set; }
		public virtual Department Department4 { get; set; }
		public virtual Department Department5 { get; set; }
		public virtual Address Address { get; set; }
		public virtual IList<Phone> Phones { get; set; } = new List<Phone>();
	}
}

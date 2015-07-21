using System;

namespace NHibernate.Test.NHSpecificTest.NH1427
{
	public class Person
	{
		public virtual int PersonId { get; set; }
		public virtual string Name { get; set; }
		public virtual string LogonId { get; set; }
		public virtual DateTime LastLogon { get; set; }
	}
}
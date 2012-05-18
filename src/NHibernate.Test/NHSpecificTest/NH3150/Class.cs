
using System.Collections.Generic;
namespace NHibernate.Test.NHSpecificTest.NH3150
{
	public class Worker
	{

		public virtual int? id { get; set; }
		
		public virtual string name { get; set; }
		public virtual string position { get; set; }
	
	}

	public class Worker2
	{

		public virtual int? id { get; set; }
		public virtual IList<Role> roles { get; set; }
	}

	public class Role
	{
		public virtual int? id { get; set; }
		public virtual string description { get; set; }
	}
}
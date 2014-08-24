using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2331
{
	public class MemberGroup
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual IList<Person> Members { get; set; }
		public virtual IList<Forum> Forums { get; set; }
	}
}

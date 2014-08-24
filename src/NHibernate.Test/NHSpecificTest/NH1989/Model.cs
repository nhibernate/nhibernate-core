using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1989
{
	public class User
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}

using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH3505
{
	public class Teacher
	{
		public virtual Guid Id { get; set; }
        public virtual int Version { get; set; }
        public virtual string Name { get; set; }
	}
}

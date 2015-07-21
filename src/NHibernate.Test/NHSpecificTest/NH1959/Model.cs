using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1959
{
	public class ClassA
	{
		public virtual Guid Id { get; set; }
		public virtual IList<ClassB> TheBag { get; set; }

		public ClassA()
		{
			TheBag = new List<ClassB>();
		}
	}

	public class ClassB 
	{
		public virtual Guid Id { get; set; }
	}
}

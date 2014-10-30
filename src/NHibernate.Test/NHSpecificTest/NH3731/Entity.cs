using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3731
{
	[Serializable]
	class Parent
	{
		public Parent()
		{
			Children = new List<Child>();
		}

		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual IList<Child> Children { get; set; }
	}

	[Serializable]
	class Child
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}
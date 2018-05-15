using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH1419
{
	public class EntityChild
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}

	public class EntityParent
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual EntityChild Child { get; set; }
		public virtual EntityChildAssigned ChildAssigned { get; set; }
	}

	public class EntityChildAssigned
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
	}
}

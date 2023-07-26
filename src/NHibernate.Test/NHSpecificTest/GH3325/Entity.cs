using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH3325
{
	public class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual int Version { get; set; } = -1;
		public virtual string Name { get; set; }
		public virtual ISet<ChildEntity> Children { get; set; } = new HashSet<ChildEntity>();
	}

	public class EntityWithoutDeleteOrphan
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ISet<ChildEntity> Children { get; set; } = new HashSet<ChildEntity>();
	}

	public class ChildEntity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}

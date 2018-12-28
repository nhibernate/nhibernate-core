using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH1754
{
	public class Entity
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ISet<ChildEntity> Children { get; set; } = new HashSet<ChildEntity>();
	}

	public class EntityWithoutDeleteOrphan
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ISet<ChildEntity> Children { get; set; } = new HashSet<ChildEntity>();
	}

	public class ChildEntity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}

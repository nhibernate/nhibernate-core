using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH3465
{
	class EntityA
	{
		public virtual Guid Id { get; set; }
		public virtual ISet<EntityB> Children { get; set; }
	}
	class EntityB
	{
		public virtual Guid Id { get; set; }
		public virtual EntityA Parent { get; set; }
	}
	class EntityC
	{
		public virtual Guid Id { get; set; }
	}
}

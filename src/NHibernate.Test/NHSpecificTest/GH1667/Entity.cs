using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH1667
{
	class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ISet<EntityChild> Children { get; set; }
	}

	class EntityChild
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}

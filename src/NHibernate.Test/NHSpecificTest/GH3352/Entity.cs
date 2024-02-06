using System;

namespace NHibernate.Test.NHSpecificTest.GH3352
{
	public class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual EntityComponentMapped Parent { get; set; }
		public virtual Component Component { get; set; }
	}

	public class EntityNameMapped : Entity
	{
	}

	public class EntityParentMapped : Entity
	{
	}

	public class EntityComponentMapped : Entity
	{
	}

	public class Component
	{
		public string Field { get; set; }

		public EntityNameMapped Entity { get; set; }
	}
}

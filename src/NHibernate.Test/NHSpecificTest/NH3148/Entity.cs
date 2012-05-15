using System;

namespace NHibernate.Test.NHSpecificTest.NH3148
{
	class EntityId
	{
		public virtual Guid Id { get; set; }
	}

	class EntityBase
	{
		public virtual EntityId ComponentId { get; set; }
		public virtual Guid Id { get; set; }
	}

	class Entity : EntityBase
	{
		public virtual string Name { get; set; }
	}
}
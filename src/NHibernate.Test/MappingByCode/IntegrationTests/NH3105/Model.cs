using System;

namespace NHibernate.Test.MappingByCode.IntegrationTests.NH3105
{
	class EntityBase
	{
		public virtual EntityId ComponentId { get; set; }
		public virtual Guid Id { get; set; }
	}
	
	class Entity : EntityBase
	{
		public virtual string Name { get; set; }
	}

	class EntityId
	{
		public virtual Guid Id { get; set; }
	}
}
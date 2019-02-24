using System.Collections.Generic;

namespace NHibernate.Test.CacheTest
{
	public class EntityInSameRegion
	{
		public virtual int Id { get; set; }
		public virtual string Description { get; set; }
		public virtual int Value { get; set; }
		public virtual ISet<EntityInSameRegion> Related { get; set; }
	}

	public class EntityA : EntityInSameRegion
	{
	}

	public class EntityB : EntityInSameRegion
	{
	}
}

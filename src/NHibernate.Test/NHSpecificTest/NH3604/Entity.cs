using System;
using System.Linq.Expressions;

namespace NHibernate.Test.NHSpecificTest.NH3604
{
	public class Entity
	{
		protected virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual EntityDetail Detail { get; set; }

		public static class PropertyAccessExpressions
		{
			public static readonly Expression<Func<Entity, Guid>> Id = x => x.Id;
		}
	}

	public class EntityDetail
	{
		// Required by NHibernate
		protected EntityDetail()
		{
		}

		public EntityDetail(Entity entity)
		{
			this.Entity = entity;
		}

		public virtual Guid Id { get; set; }
		protected virtual Entity Entity { get; set; }
		public virtual string ExtraInfo { get; set; }

		public static class PropertyAccessExpressions
		{
			public static readonly Expression<Func<EntityDetail, Entity>> Entity = x => x.Entity;
		}
	}
}
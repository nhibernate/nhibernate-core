using System;
using System.Collections.Generic;

namespace NHibernate.Test.MappingByCode.IntegrationTests.NH3135
{
	public abstract class EntityBase_WithComponentCollection
	{
		protected EntityBase_WithComponentCollection()
		{
			ComponentCollection = new List<Component>();
		}
		public virtual Guid Id { get; private set; }
		public virtual ICollection<Component> ComponentCollection { get; private set; }
	}
	public class Component
	{
		public virtual string Property1 { get; set; }
		public virtual int Property2 { get; set; }
	}
	public class Entity1 : EntityBase_WithComponentCollection
	{
		public virtual int Foo { get; set; }
	}
	public class Entity2 : EntityBase_WithComponentCollection
	{
		public virtual int Bar { get; set; }
	}
	public class Entity3 : EntityBase_WithComponentCollection
	{
		public virtual int Baz { get; set; }
	}
}

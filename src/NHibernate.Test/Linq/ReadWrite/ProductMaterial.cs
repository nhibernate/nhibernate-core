using System;

namespace NHibernate.Test.Linq.ReadWrite
{
	public class EntityVersionedWithName<TKey> : EntityVersioned<TKey>
		where TKey: struct, IComparable<TKey>
	{
		public virtual string Name { get; set; }
	}

	public class Material : EntityVersionedWithName<int>
	{
		public virtual MaterialDefinition MaterialDefinition { get; set; }

		public virtual ProductDefinition ProductDefinition { get; set; }
	}

	public class MaterialDefinition : EntityVersionedWithName<int>
	{
	}

	public class ProductDefinition : EntityVersionedWithName<int>
	{
		public virtual MaterialDefinition MaterialDefinition { get; set; }
	}
}

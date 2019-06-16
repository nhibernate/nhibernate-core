using System;

namespace NHibernate.Test.Hql.EntityJoinHqlTestEntities
{
	public class EntityComplex
	{
		public virtual Guid Id { get; set; }

		public virtual int Version { get; set; }

		public virtual string Name { get; set; }

		public virtual string LazyProp { get; set; }

		public virtual EntityComplex SameTypeChild { get; set; }
		public virtual EntityComplex SameTypeChild2 { get; set; }

	}

	public class EntityWithCompositeId
	{
		public virtual CompositeKey Key { get; set; }
		public virtual string Name { get; set; }
	}
	public class CompositeKey
	{
		public int Id1 { get; set; }
		public int Id2 { get; set; }

		public override bool Equals(object obj)
		{
			var key = obj as CompositeKey;
			return key != null
					&& Id1 == key.Id1
					&& Id2 == key.Id2;
		}

		public override int GetHashCode()
		{
			var hashCode = -1596524975;
			hashCode = hashCode * -1521134295 + Id1.GetHashCode();
			hashCode = hashCode * -1521134295 + Id2.GetHashCode();
			return hashCode;
		}
	}

	public class EntityCustomEntityName
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}

	public class EntityWithNoAssociation
	{
		public virtual Guid Id { get; set; }
		public virtual Guid Complex1Id { get; set; }
		public virtual Guid Complex2Id { get; set; }
		public virtual Guid Simple1Id { get; set; }
		public virtual Guid Simple2Id { get; set; }
		public virtual int Composite1Key1 { get; set; }
		public virtual int Composite1Key2 { get; set; }
		public virtual Guid CustomEntityNameId { get; set; }
		public virtual string Name { get; set; }
	}
}

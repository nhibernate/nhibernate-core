using System;
using System.Collections.Generic;

namespace NHibernate.Test.Criteria
{
	public class EntitySimpleChild
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
	
	public class EntityCustomEntityName
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}

	public class EntityComplex
	{
		public virtual Guid Id { get; set; }

		public virtual int Version { get; set; }

		public virtual string Name { get; set; }

		public virtual string LazyProp { get; set; }

		public virtual EntitySimpleChild Child1 { get; set; }
		public virtual EntitySimpleChild Child2 { get; set; }
		public virtual EntityComplex SameTypeChild { get; set; }

		public virtual IList<EntitySimpleChild> ChildrenList { get; set; }
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

	public class EntityWithCompositeId
	{
		public virtual CompositeKey Key { get; set; }
		public virtual string Name { get; set; }
	}
}

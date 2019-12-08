using System;
using System.Collections.Generic;

namespace NHibernate.Test.Futures
{
	public class EntityComplex
	{
		public virtual Guid Id { get; set; }

		public virtual int Version { get; set; }

		public virtual string Name { get; set; }

		public virtual string LazyProp { get; set; }

		public virtual EntitySimpleChild Child1 { get; set; }
		public virtual EntitySimpleChild Child2 { get; set; }
		public virtual EntityComplex SameTypeChild { get; set; }

		public virtual IList<EntitySimpleChild> ChildrenList { get; set; } = new List<EntitySimpleChild>();
		public virtual IList<EntityComplex> ChildrenListEmpty { get; set; } = new List<EntityComplex>();
	}

	public class EntitySimpleChild
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual EntityComplex Parent { get; set; }
	}

	public class EntitySubselectChild
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual EntityEager Parent { get; set; }
	}

	public class EntityEagerChild
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
	}
	
	public class EntityEager
	{
		public Guid Id { get; set; }
		public string Name { get; set; }

		public EntityEagerChild EagerEntity { get; set; }
		public IList<EntitySubselectChild> ChildrenListSubselect { get; set; }
		public IList<EntitySimpleChild> ChildrenListEager { get; set; } //= new HashSet<EntitySimpleChild>();
	}
}

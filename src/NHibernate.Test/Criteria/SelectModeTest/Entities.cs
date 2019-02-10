using System;
using System.Collections.Generic;

namespace NHibernate.Test.Criteria.SelectModeTest
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

	public class EntityEager
	{
		public Guid Id { get; set; }
		public int Version { get; set; }
		public string Name { get; set; }
		public IList<EntityEagerChild> ChildrenList { get; set; }
	}

	public class EntityEagerChild :BaseChild
	{
	}

	public abstract class BaseChild
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Guid? ParentId { get; set; }
	}
	public class EntitySimpleChild : BaseChild
	{
		public virtual IList<Level2Child> Children { get; set; } = new List<Level2Child>();
		public virtual string LazyProp { get; set; }
		public virtual int OrderIdx { get; set; }
	}

	public class Level2Child : BaseChild
	{
		public virtual IList<Level3Child> Children { get; set; } = new List<Level3Child>();
	}

	public class Level3Child : BaseChild
	{ }
}

using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH3334
{
	public class Entity
	{
		public virtual int Id { get; set; }
 		public virtual string Name { get; set; }
        public virtual ISet<ChildEntity> Children { get; set; } = new HashSet<ChildEntity>();
        public virtual OtherEntity OtherEntity { get; set; }
	}

	public class ChildEntity
	{
		public virtual int Id { get; set; }
		public virtual Entity Parent { get; set; }
		public virtual string Name { get; set; }
		public virtual GrandChildEntity Child { get; set; }
	}

	public class GrandChildEntity
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
	}

	public class OtherEntity
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ISet<Entity> Entities { get; set; } = new HashSet<Entity>();
	}
}

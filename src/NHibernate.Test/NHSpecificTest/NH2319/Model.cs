using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2319
{
	class Parent
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ICollection<Child> Children { get; set; } = new List<Child>();
		public virtual IDictionary<Guid, Child> ChildrenMap { get; set; } = new Dictionary<Guid, Child>();
	}

	class Child
	{
		public virtual Parent Parent { get; set; }
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ICollection<GrandChild> GrandChildren { get; set; } = new List<GrandChild>();
		public virtual ICollection<Parent> Parents { get; set; } = new List<Parent>();
	}
	class GrandChild
	{
		public virtual Child Child { get; set; }
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ICollection<Child> ParentChidren { get; set; } = new List<Child>();
	}
}

using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH3317
{
	public class Entity
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public IList<ComponentListEntry> Entries { get; set; } = new List<ComponentListEntry>();
	}

	public class ComponentListEntry
	{
		public string DummyString { get; set; }
		public EntityWithParent ComponentReference { get; set; }
	}

	public class EntityWithParent
	{
		public Guid Id { get; set; }
		public ParentEntity Parent { get; set; }
	}

	public class ParentEntity
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
	}
}

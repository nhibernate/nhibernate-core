using System.Collections.Generic;

// ReSharper disable CollectionNeverUpdated.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

namespace NHibernate.Test.NHSpecificTest.GH3643
{
	class Entity
	{
		private readonly ICollection<ChildEntity> _children = [];
		public virtual EntityId Id { get; set; }
		public virtual ICollection<ChildEntity> Children => _children;
	}

	class ChildEntity
	{
		public virtual int Id { get; set; }
	}

	enum EntityId
	{
		Id1,
		Id2
	}
}

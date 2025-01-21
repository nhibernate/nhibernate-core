using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable CollectionNeverUpdated.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

namespace NHibernate.Test.NHSpecificTest.GH3643
{
	class Entity
	{
		private readonly ICollection<ChildEntity> _children = new List<ChildEntity>();
		public virtual EntityId Id { get; protected set; }
		public virtual IEnumerable<ChildEntity> Children => _children.AsEnumerable();
	}

	class ChildEntity
	{
		public virtual int Id { get; protected set; }
	}

	enum EntityId
	{
		Id1,
		Id2
	}
}

using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH3288
{
	class TopEntity
	{
		public virtual int Id { get; set; }
		public virtual MiddleEntity MiddleEntity { get; set; }
	}
	class MiddleEntity
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ISet<Component> Components { get; set; } = new HashSet<Component>();
	}

	class Component
	{
		public virtual MiddleEntity MiddleEntity { get; set; }
		public virtual int Value { get; set; }

		public override bool Equals(object obj)
		{
			return (obj as Component)?.MiddleEntity.Id == MiddleEntity.Id;
		}

		public override int GetHashCode()
		{
			return MiddleEntity.Id.GetHashCode();
		}
	}
}

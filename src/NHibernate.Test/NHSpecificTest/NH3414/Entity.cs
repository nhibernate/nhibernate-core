using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3414
{
	public class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual int SomeValue { get; set; }

		public virtual Entity Parent { get; set; }

		private readonly IList<Entity> children = new List<Entity>();
		public virtual IEnumerable<Entity> Children { get { return children; } }

		public virtual void AddChild(Entity child)
		{
			children.Add(child);
			child.Parent = this;
		}

		public override string ToString()
		{
			return Name + SomeValue;
		}

		public override bool Equals(object obj)
		{
			var other = obj as Entity;
			if (other == null)
			{
				return false;
			}
			return other.Id == Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH3316
{
	class Entity
	{
		public Entity()
		{
			Children = new HashSet<ChildComponent>();    
		}

		public virtual Guid Id { get; set; }

		public virtual ICollection<ChildComponent> Children { get; protected set; }

		public virtual void AddChild(int value)
		{
			Children.Add(new ChildComponent(this, value));
		}

		public override bool Equals(object obj)
		{
			Entity other = obj as Entity;
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;

			return Id == other.Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}

	class ChildComponent
	{
		protected ChildComponent()
		{
		}

		public ChildComponent(Entity parent, int value)
		{
			Parent = parent;
			Value = value;
		}

		public virtual Entity Parent { get; protected set; }

		public virtual int Value { get; protected set; }

		public override bool Equals(object obj)
		{
			ChildComponent other = obj as ChildComponent;
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;

			return Parent == other.Parent && Value == other.Value;
		}

		public override int GetHashCode()
		{
			int hash = 17;
			unchecked
			{
				hash = (hash * 23) + Parent.GetHashCode();
				hash = (hash * 23) + Value.GetHashCode();
			}

			return hash;
		}
	}
}
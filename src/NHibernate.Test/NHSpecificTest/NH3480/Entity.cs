using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3480
{
	class Entity
	{
		public Entity()
		{
			Children = new HashSet<Child>();
		}

		public virtual Key Id { get; set; }
		public virtual string Name { get; set; }
		public virtual int OtherId { get; set; }
		public virtual ISet<Child> Children { get; set; }

		public override bool Equals(object obj)
		{
			if(obj is Entity)
			{
				var otherEntity = (Entity)obj;
				return otherEntity.Id.Equals(this.Id);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public class Key
		{
			public virtual Guid Id { get; set; }

			public override bool Equals(object obj)
			{
				if (obj is Key)
				{
					var otherEntity = (Key)obj;
					return otherEntity.Id.Equals(this.Id);
				}
				return false;
			}

			public override int GetHashCode()
			{
				// Needed to reproduce the problem
				return 20.GetHashCode();
			}
		}
	}

	class Child
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Entity Parent { get; set; }
	}
}
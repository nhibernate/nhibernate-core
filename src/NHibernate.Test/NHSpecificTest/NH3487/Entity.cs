using System;
using Iesi.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3487
{
	[Serializable()]
	public class Entity
	{
		public virtual Key Id { get; set; }
		public virtual string Name { get; set; }

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
	}

	[Serializable()]
	public class Key
	{
		public virtual int Id { get; set; }

		public override bool Equals(object obj)
		{
			if (obj is Key)
			{
				var otherEntity = (Key)obj;
				return otherEntity.Id == this.Id;
			}
			return false;
		}

		public override int GetHashCode()
		{
			// Important to reproduce the problem - forces the keys to collide in the hash map
			return 1;
		}
	}
}
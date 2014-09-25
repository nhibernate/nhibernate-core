using System;

namespace NHibernate.Test.NHSpecificTest.NH3487
{
	[Serializable]
	public class Entity
	{
		public virtual Key Id { get; set; }
		public virtual string Name { get; set; }

		public override bool Equals(object obj)
		{
			var otherEntity = obj as Entity;
			if (otherEntity != null)
			{
				return otherEntity.Id.Equals(Id);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}

	[Serializable]
	public class Key
	{
		public virtual int Id { get; set; }

		public override bool Equals(object obj)
		{
			var otherEntity = obj as Key;
			if (otherEntity != null)
			{
				return otherEntity.Id == Id;
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
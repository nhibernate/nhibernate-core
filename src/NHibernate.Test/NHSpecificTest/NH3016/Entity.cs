using System;

namespace NHibernate.Test.NHSpecificTest.NH3016
{
	public class Entity
	{
		// nested class
		public class Key
		{
			public virtual Guid Id { get; set; }

			public override bool Equals(object obj)
			{
				if (obj == null) return false;
				return Id.Equals(((Key)obj).Id);
			}

			public override int GetHashCode()
			{
				return Id.GetHashCode();
			}
		}

		public virtual Key Id { get; set; }

		public virtual string Name { get; set; }
	}
}
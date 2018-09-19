using System;

namespace NHibernate.Test.Insertordering.AnimalModel
{
	public class Animal
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Person Owner { get; set; }

		public override bool Equals(object obj)
		{
			return (obj as Animal)?.Id == Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}

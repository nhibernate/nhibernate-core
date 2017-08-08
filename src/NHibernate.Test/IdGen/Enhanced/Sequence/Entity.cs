namespace NHibernate.Test.IdGen.Enhanced.Sequence
{
	public class Entity
	{
		// Assigned by reflection
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value 0
		private long _id;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value 0

		public virtual long Id
		{
			get { return _id; }
		}

		public virtual string Name
		{
			get; set;
		}
		
		public Entity() { }

		public Entity(string name)
		{
			Name = name;
		}
	}
}
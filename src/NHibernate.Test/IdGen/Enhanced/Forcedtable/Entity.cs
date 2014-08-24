namespace NHibernate.Test.IdGen.Enhanced.Forcedtable
{
	public class Entity
	{

#pragma warning disable 0414  // unassigned variable
		private long _id;
#pragma warning restore 0414

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
namespace NHibernate.Test.NHSpecificTest.GH3176
{
	public class Entity
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Component Component { get; set; }
		public virtual int Version { get; set; }
	}

	public class Component
	{
		public virtual string Field { get; set; }
	}
}

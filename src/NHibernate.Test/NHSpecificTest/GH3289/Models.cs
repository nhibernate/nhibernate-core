namespace NHibernate.Test.NHSpecificTest.GH3289
{
	public interface IEntity
	{
		int Id { get; set; }
		string Name { get; set; }
		Component Component { get; set; }
	}

	public interface ISubEntity : IEntity
	{
		public bool SomeProperty { get; set; }
	}

	public class Entity : IEntity
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Component Component { get; set; }
	}
	public class OtherEntity : IEntity
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Component Component { get; set; }
	}

	public class SubEntity : Entity, ISubEntity
	{
		public virtual bool SomeProperty { get; set; }
	}

	public class Component
	{
		public virtual string Field { get; set; }
	}
}

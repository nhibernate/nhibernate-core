namespace NHibernate.Test.NHSpecificTest.GH1628
{
	public interface IEntity
	{
		object Thing { get; }
	}

	public class Entity : IEntity
	{
		public virtual int Id { get; set; }

		object IEntity.Thing { get { return null; } }
		public virtual string Name { get; set; }
		public virtual string ALongText { get; set; }
	}
}

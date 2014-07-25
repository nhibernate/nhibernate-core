
namespace NHibernate.Test.NHSpecificTest.NH3641
{
	public interface IEntity
	{
		int Id { get; set; }
		bool Flag { get; set; }
		Entity ChildConcrete { get; set; }
		IEntity ChildInterface { get; set; }
	}

	public class Entity : IEntity
	{
		public virtual int Id { get; set; }
		public virtual bool Flag { get; set; }
		public virtual Entity ChildConcrete { get; set; }
		public virtual IEntity ChildInterface { get; set; }
	}
}

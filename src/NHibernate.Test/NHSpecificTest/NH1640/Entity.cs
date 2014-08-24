namespace NHibernate.Test.NHSpecificTest.NH1640
{
	public class Entity
	{
		public virtual int Id { get; set; }

		public virtual string Name { get; set; }

		public virtual Entity Child { get; set; }
	}
}
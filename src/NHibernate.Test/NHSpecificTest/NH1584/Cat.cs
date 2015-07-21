namespace NHibernate.Test.NHSpecificTest.NH1584
{
	public abstract class Cat
	{
		public virtual int Id { get; protected set; }

		public virtual string Name { get; set; }
	}
}
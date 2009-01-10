namespace NHibernate.Test.NHSpecificTest.NH1584
{
	public abstract class Cat
	{
		public virtual int Id { get; private set; }

		public virtual string Name { get; set; }
	}
}
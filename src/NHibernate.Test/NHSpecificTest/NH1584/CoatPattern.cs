namespace NHibernate.Test.NHSpecificTest.NH1584
{
	public abstract class CoatPattern
	{
		public virtual int Id { get; private set; }

		public virtual string Description { get; set; }
	}
}
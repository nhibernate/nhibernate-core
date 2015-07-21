namespace NHibernate.Test.NHSpecificTest.NH2303
{
	public abstract class Actor
	{
		public virtual int Id { get; set; }
	}

	public class Person : Actor
	{
	}

	public abstract class Role : Actor
	{
		public virtual Person Performer { get; set; }
	}

	public class Developer : Role
	{
	}
}